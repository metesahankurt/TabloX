using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        // Siparişi tamamlama (Checkout)
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _context.CartItems.Include(c => c.Artwork).Where(c => c.UserId == userId).ToListAsync();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Sepetiniz boş.";
                return RedirectToAction("Index", "Cart");
            }
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Items = cartItems.Select(c => new OrderItem
                {
                    ArtworkId = c.ArtworkId,
                    Quantity = c.Quantity
                }).ToList()
            };
            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Siparişiniz başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }
    }
} 