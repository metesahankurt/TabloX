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
            var orders = await _context.Orders.Include(o => o.Items).ThenInclude(i => i.Artwork).Where(o => o.UserId == userId).OrderByDescending(o => o.OrderDate).ToListAsync();
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
            ViewBag.CartItems = cartItems;
            ViewBag.Total = cartItems.Sum(i => i.Artwork.Price * i.Quantity);
            return View(new CheckoutViewModel());
        }

        // Siparişi tamamlama (Checkout) - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model, int? artworkId)
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
            if (!ModelState.IsValid)
            {
                ViewBag.CartItems = cartItems;
                ViewBag.Total = cartItems.Sum(i => i.Artwork.Price * i.Quantity);
                return View(model);
            }
            // Sipariş numarası üret
            string orderNumber = $"TBL-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            // Maskeli kart numarası
            string maskedCard = new string('*', model.CardNumber.Length - 4) + model.CardNumber[^4..];
            // Siparişi oluştur
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                OrderNumber = orderNumber,
                ShippingAddress = model.ShippingAddress,
                MaskedCardNumber = maskedCard,
                ShippingStatus = "Siparişiniz hazırlanıyor",
                Items = new List<OrderItem>()
            };
            foreach (var item in cartItems)
            {
                order.Items.Add(new OrderItem
                {
                    ArtworkId = item.ArtworkId,
                    Artwork = item.Artwork,
                    Quantity = item.Quantity,
                    Order = order
                });
            }
            _context.Orders.Add(order);
            if (!artworkId.HasValue)
                _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Satın alma işleminiz başarıyla tamamlandı.";
            return RedirectToAction("Index");
        }
    }
} 