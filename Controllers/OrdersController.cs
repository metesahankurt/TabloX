using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TabloX2.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sipariş geçmişi
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Artwork)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        // Siparişi tamamlama (Checkout) - GET
        [HttpGet]
        public async Task<IActionResult> Checkout(int? artworkId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<CartItem> cartItems;
            if (artworkId.HasValue)
            {
                var artwork = await _context.Artworks.FindAsync(artworkId.Value);
                if (artwork == null)
                {
                    TempData["Error"] = "Eser bulunamadı.";
                    return RedirectToAction("Index", "Home");
                }
                cartItems = new List<CartItem> {
                    new CartItem { Artwork = artwork, ArtworkId = artwork.Id, Quantity = 1, UserId = userId }
                };
                ViewBag.DirectBuy = true;
            }
            else
            {
                cartItems = await _context.CartItems.Include(c => c.Artwork).Where(c => c.UserId == userId).ToListAsync();
                if (!cartItems.Any())
                {
                    TempData["Error"] = "Sepetiniz boş.";
                    return RedirectToAction("Index", "Cart");
                }
                ViewBag.DirectBuy = false;
            }

            var initialTotalAmount = cartItems.Sum(i => i.Artwork.Price * i.Quantity);
            ViewBag.CartItems = cartItems;
            ViewBag.TotalAmount = initialTotalAmount;
            ViewBag.DiscountAmount = 0;
            ViewBag.FinalAmount = initialTotalAmount;

            return View(new CheckoutViewModel { PaymentMethod = PaymentMethod.CreditCard });
        }

        // Siparişi tamamlama (Checkout) - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model, int? artworkId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<CartItem> cartItems;

            // Sepetteki ürünleri al
            if (artworkId.HasValue)
            {
                var artwork = await _context.Artworks.FindAsync(artworkId.Value);
                if (artwork == null)
                {
                    TempData["Error"] = "Eser bulunamadı.";
                    return RedirectToAction("Index", "Home");
                }
                cartItems = new List<CartItem> {
                    new CartItem { Artwork = artwork, ArtworkId = artwork.Id, Quantity = 1, UserId = userId }
                };
                ViewBag.DirectBuy = true;
            }
            else
            {
                cartItems = await _context.CartItems
                    .Include(ci => ci.Artwork)
                    .Where(ci => ci.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    TempData["Error"] = "Sepetiniz boş.";
                    return RedirectToAction("Index", "Cart");
                }
                ViewBag.DirectBuy = false;
            }

            // ViewBag değerlerini set et
            var totalAmount = cartItems.Sum(i => i.Artwork.Price * i.Quantity);
            ViewBag.CartItems = cartItems;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.DiscountAmount = 0;
            ViewBag.FinalAmount = totalAmount;

            // Form validasyonu
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm gerekli alanları doldurunuz.";
                return View(model);
            }

            // Ödeme yöntemi validasyonu
            if (!ValidatePaymentMethod(model))
            {
                return View(model);
            }

            GiftCard? giftCard = null;
            if (model.PaymentMethod == PaymentMethod.GiftCard)
            {
                // Önce test kodlarını kontrol et
                if (IsValidGiftCard(model.GiftCardCode))
                {
                    // Test hediye kartı için sahte bir GiftCard nesnesi oluştur
                    var discountPercentage = GetGiftCardDiscountPercentage(model.GiftCardCode);
                    giftCard = new GiftCard
                    {
                        Code = model.GiftCardCode,
                        IsActive = true,
                        Amount = (totalAmount * discountPercentage) / 100
                    };
                }
                else
                {
                    // Veritabanından kontrol et
                    giftCard = await _context.GiftCards
                        .FirstOrDefaultAsync(g => g.Code == model.GiftCardCode && g.IsActive);
                }

                if (giftCard == null)
                {
                    ModelState.AddModelError("GiftCardCode", "Geçersiz hediye kartı kodu");
                    return View(model);
                }
            }

            try
            {
                // Sipariş numarası üret
                string orderNumber = GenerateOrderNumber();

                // Toplam tutarı hesapla
                var orderTotalAmount = cartItems.Sum(i => i.Artwork.Price * i.Quantity);
                var discountAmount = 0m;
                var finalAmount = orderTotalAmount;

                // Hediye kartı indirimi
                if (model.PaymentMethod == PaymentMethod.GiftCard && giftCard != null)
                {
                    discountAmount = giftCard.Amount;
                    finalAmount = Math.Max(0, orderTotalAmount - discountAmount);

                    // Hediye kartını kullanıldı olarak işaretle
                    giftCard.IsActive = false;
                    giftCard.UsedDate = DateTime.Now;
                    giftCard.UsedBy = userId;
                }

                // Siparişi oluştur
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    OrderNumber = orderNumber,
                    ShippingAddress = model.ShippingAddress ?? "",
                    PaymentMethod = model.PaymentMethod,
                    InstallmentCount = model.InstallmentCount,
                    CryptoWalletAddress = model.CryptoWalletAddress,
                    BankAccountNumber = model.BankAccountNumber,
                    GiftCardCode = model.GiftCardCode,
                    ShippingStatus = "Siparişiniz hazırlanıyor",
                    TotalAmount = orderTotalAmount,
                    DiscountAmount = discountAmount,
                    FinalAmount = finalAmount
                };

                // Sipariş öğelerini oluştur
                order.Items = cartItems.Select(item => new OrderItem
                {
                    ArtworkId = item.ArtworkId,
                    Artwork = item.Artwork,
                    Quantity = item.Quantity,
                    Order = order
                }).ToList();

                // Kredi kartı bilgilerini maskele
                if (model.PaymentMethod == PaymentMethod.CreditCard && !string.IsNullOrEmpty(model.CardNumber))
                {
                    var cardNumber = model.CardNumber.Replace(" ", "");
                    order.MaskedCardNumber = cardNumber.Length > 4 
                        ? new string('*', cardNumber.Length - 4) + cardNumber[^4..]
                        : "****";
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Orders.Add(order);

                    if (!artworkId.HasValue)
                    {
                        _context.CartItems.RemoveRange(cartItems);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Satın alma işleminiz başarıyla tamamlandı.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Sipariş işlemi sırasında bir hata oluştu: {ex.Message}");
                return View(model);
            }
        }

        private bool ValidatePaymentMethod(CheckoutViewModel model)
        {
            switch (model.PaymentMethod)
            {
                case PaymentMethod.CreditCard:
                    if (string.IsNullOrEmpty(model.CardNumber) || string.IsNullOrEmpty(model.CVV) || 
                        string.IsNullOrEmpty(model.Expiry) || string.IsNullOrEmpty(model.CardHolder))
                    {
                        ModelState.AddModelError("", "Kredi kartı bilgileri eksik");
                        return false;
                    }
                    break;

                case PaymentMethod.BankTransfer:
                    if (string.IsNullOrEmpty(model.BankAccountNumber))
                    {
                        ModelState.AddModelError("", "Banka hesap numarası gerekli");
                        return false;
                    }
                    break;

                case PaymentMethod.CryptoCurrency:
                    if (string.IsNullOrEmpty(model.CryptoWalletAddress))
                    {
                        ModelState.AddModelError("", "Kripto cüzdan adresi gerekli");
                        return false;
                    }
                    break;

                case PaymentMethod.GiftCard:
                    if (string.IsNullOrEmpty(model.GiftCardCode))
                    {
                        ModelState.AddModelError("", "Hediye kartı kodu gerekli");
                        return false;
                    }
                    break;
            }

            return true;
        }

        private string GenerateOrderNumber()
        {
            return $"TBL-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }

        // Geçerli hediye kartı kodlarını kontrol et
        private bool IsValidGiftCard(string code)
        {
            // Test için geçerli hediye kartı kodları
            var validGiftCards = new Dictionary<string, decimal>
            {
                { "GIFT-2024-DEMO-100X", 100 }, // %100 indirim
                { "GIFT-2024-TEST-50XX", 50 },  // %50 indirim
                { "GIFT-2024-SAVE-25XX", 25 }   // %25 indirim
            };

            return validGiftCards.ContainsKey(code);
        }

        // Hediye kartı indirim oranını al
        private decimal GetGiftCardDiscountPercentage(string code)
        {
            var validGiftCards = new Dictionary<string, decimal>
            {
                { "GIFT-2024-DEMO-100X", 100 },
                { "GIFT-2024-TEST-50XX", 50 },
                { "GIFT-2024-SAVE-25XX", 25 }
            };

            return validGiftCards.GetValueOrDefault(code, 0);
        }
    }
} 