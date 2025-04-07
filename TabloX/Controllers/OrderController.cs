using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX.Data;
using TabloX.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TabloX.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Message"] = "Sepetiniz boş.";
                return RedirectToAction("Index", "Cart");
            }

            // Toplam tutarı hesapla
            decimal totalAmount = cart.Items
                .Where(ci => ci.Product != null)
                .Sum(ci => ci.Quantity * ci.Product.Price);

            // Yeni sipariş oluştur ve gerekli alanları doldur
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = totalAmount,
                OrderCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
            };

            ViewData["CartTotal"] = totalAmount;
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Message"] = "Sepetiniz boş.";
                return RedirectToAction("Index", "Cart");
            }

            // Model validasyonunu kontrol et
            if (!ModelState.IsValid)
            {
                ViewData["CartTotal"] = cart.Items.Sum(ci => ci.Quantity * ci.Product.Price);
                return View(order);
            }

            try
            {
                // Sipariş öğelerini oluştur
                var orderItems = cart.Items
                    .Where(ci => ci.Product != null)
                    .Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.Product.Price
                    }).ToList();

                // Sipariş nesnesini güncelle
                order.Status = "Completed";
                order.OrderDate = DateTime.Now;
                order.UserId = user.Id;
                order.TotalAmount = cart.Items.Sum(ci => ci.Quantity * ci.Product.Price);
                order.OrderItems = orderItems;

                // Siparişi veritabanına kaydet
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Sepeti temizle
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();

                // Sipariş bilgilerini TempData'ya kaydet (decimal'i string'e çevirerek)
                TempData["OrderCode"] = order.OrderCode;
                TempData["OrderAmount"] = order.TotalAmount.ToString("N2");
                TempData["OrderSuccess"] = "true";

                // Başarı sayfasına yönlendir
                return RedirectToAction("OrderSuccess");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Sipariş oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
                ViewData["CartTotal"] = cart.Items.Sum(ci => ci.Quantity * ci.Product.Price);
                return View(order);
            }
        }

        public IActionResult OrderSuccess()
        {
            // TempData kontrolü
            if (TempData["OrderSuccess"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Sipariş bilgilerini ViewBag'e aktar
            ViewBag.OrderCode = TempData["OrderCode"];
            ViewBag.OrderAmount = TempData["OrderAmount"]; // Artık string olarak gelecek

            return View();
        }
    }
} 