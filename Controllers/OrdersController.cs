using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Sipariş geçmişi
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Artwork)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Artwork)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Artwork)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var subtotal = cart.Items.Sum(i => (i.Artwork?.Price ?? 0) * i.Quantity);
            var tax = subtotal * 0.18M;
            var total = subtotal + tax;

            var model = new CheckoutViewModel
            {
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Email = user.Email ?? "",
                Phone = user.PhoneNumber ?? "",
                Address = user.Address ?? "",
                City = user.City ?? "",
                District = user.District ?? "",
                PostalCode = user.PostalCode ?? "",
                PaymentMethod = "CreditCard",
                SubTotal = subtotal,
                Tax = tax,
                Total = total
            };

            ViewBag.Cart = cart;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Sadece kredi kartı validasyonu
            if (string.IsNullOrWhiteSpace(model.CardNumber) || !LuhnCheck(model.CardNumber.Replace(" ", "")))
                ModelState.AddModelError("CardNumber", "Geçerli bir kredi kartı numarası giriniz.");
            if (string.IsNullOrWhiteSpace(model.CardExpiry) || !System.Text.RegularExpressions.Regex.IsMatch(model.CardExpiry, @"^(0[1-9]|1[0-2])\/\d{2}$"))
                ModelState.AddModelError("CardExpiry", "Geçerli bir son kullanma tarihi giriniz (AA/YY).");
            if (string.IsNullOrWhiteSpace(model.CardCvv) || !System.Text.RegularExpressions.Regex.IsMatch(model.CardCvv, @"^\d{3,4}$"))
                ModelState.AddModelError("CardCvv", "Geçerli bir CVV giriniz.");
            if (string.IsNullOrWhiteSpace(model.CardHolder))
                ModelState.AddModelError("CardHolder", "Kart üzerindeki isim zorunludur.");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Artwork)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var subtotal = cart.Items.Sum(i => (i.Artwork?.Price ?? 0) * i.Quantity);
            var tax = subtotal * 0.18M;
            var total = subtotal + tax;

            var order = new Order
            {
                UserId = user.Id,
                OrderNumber = GenerateOrderNumber(),
                OrderDate = DateTime.UtcNow,
                ShippingAddress = $"{model.Address}\n{model.District}, {model.City} {model.PostalCode}",
                PhoneNumber = model.Phone,
                ShippingStatus = "Hazırlanıyor",
                Status = OrderStatus.Pending,
                TotalAmount = total,
                DiscountAmount = 0,
                FinalAmount = total
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart.Items)
            {
                if (item.Artwork != null)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ArtworkId = item.ArtworkId,
                        Quantity = item.Quantity,
                        Price = item.Artwork.Price,
                        Order = order,
                        Artwork = item.Artwork
                    };
                    _context.OrderItems.Add(orderItem);
                }
            }

            // Sepeti temizle
            _context.CartItems.RemoveRange(cart.Items);
            cart.Items.Clear();
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Artwork)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        private string GenerateOrderNumber()
        {
            return $"TBL-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }

        // Luhn algoritması ile kredi kartı kontrolü
        private bool LuhnCheck(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(cardNumber[i].ToString());
                if (alternate)
                {
                    n *= 2;
                    if (n > 9) n -= 9;
                }
                sum += n;
                alternate = !alternate;
            }
            return (sum % 10 == 0);
        }
    }
} 