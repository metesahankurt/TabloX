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

            decimal totalAmount = 0;
            foreach (var item in cart.Items)
            {
                if (item.Product != null)
                {
                    totalAmount += item.Quantity * item.Product.Price;
                }
            }

            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = "Pending",
                OrderItems = cart.Items
                    .Where(ci => ci.Product != null)
                    .Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.Product.Price
                    }).ToList()
            };

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

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

            // Sipariş kodu oluştur
            string orderCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in cart.Items)
            {
                if (item.Product != null)
                {
                    totalAmount += item.Quantity * item.Product.Price;
                    orderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price
                    });
                }
            }

            // Siparişi oluştur
            var newOrder = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                Status = "Completed",
                TotalAmount = totalAmount,
                ShippingAddress = order.ShippingAddress,
                BillingAddress = order.BillingAddress,
                CardHolderName = order.CardHolderName,
                CardNumber = order.CardNumber,
                ExpiryDate = order.ExpiryDate,
                CVV = order.CVV,
                OrderItems = orderItems,
                OrderCode = orderCode
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // Sepeti temizle
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();

            // Sipariş kodunu TempData'ya kaydet
            TempData["OrderCode"] = orderCode;
            TempData.Keep("OrderCode"); // TempData değerini koru
            
            return RedirectToAction("OrderSuccess", new { orderCode = orderCode });
        }

        public IActionResult OrderSuccess(string orderCode)
        {
            if (string.IsNullOrEmpty(orderCode))
            {
                orderCode = TempData["OrderCode"]?.ToString();
                if (string.IsNullOrEmpty(orderCode))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            
            ViewBag.OrderCode = orderCode ?? string.Empty;
            return View();
        }
    }
} 