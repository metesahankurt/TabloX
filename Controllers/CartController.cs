using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using System.Security.Claims;

namespace TabloX2.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Artwork)
                .ThenInclude(a => a.Artist)
                .Include(c => c.Items)
                .ThenInclude(i => i.Artwork)
                .ThenInclude(a => a.Category)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new Cart { UserId = user.Id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return View(cart.Items);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int artworkId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Response.StatusCode = 401;
                return Json(new { success = false, message = "Kullanıcı bulunamadı. Lütfen tekrar giriş yapın." });
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new Cart { UserId = user.Id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var artwork = await _context.Artworks.FindAsync(artworkId);
            if (artwork == null)
            {
                return Json(new { success = false, message = "Eser bulunamadı." });
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ArtworkId == artworkId);
            if (existingItem != null)
            {
                return Json(new { success = false, message = "Bu eser zaten sepetinizde bulunuyor." });
            }

            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ArtworkId = artworkId,
                UserId = user.Id,
                Quantity = 1
            };

            cart.Items.Add(cartItem);
            await _context.SaveChangesAsync();

            var cartCount = cart.Items.Count;
            return Json(new { success = true, cartCount });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int artworkId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı. Lütfen tekrar giriş yapın." });
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                return Json(new { success = false, message = "Sepet bulunamadı." });
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ArtworkId == artworkId);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Ürün sepette bulunamadı." });
            }

            cart.Items.Remove(cartItem);
            await _context.SaveChangesAsync();

            var cartCount = cart.Items.Count;
            return Json(new { success = true, cartCount });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = true, cartCount = 0 });
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            var cartCount = cart?.Items.Count ?? 0;
            return Json(new { success = true, cartCount });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int artworkId, int quantity)
        {
            if (quantity < 1)
            {
                return Json(new { success = false, message = "Miktar 1'den küçük olamaz." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı. Lütfen tekrar giriş yapın." });
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                return Json(new { success = false, message = "Sepet bulunamadı." });
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ArtworkId == artworkId);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Ürün sepette bulunamadı." });
            }

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartSummary()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı. Lütfen tekrar giriş yapın." });
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Artwork)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                return Json(new { success = true, subtotal = "0", tax = "0", total = "0" });
            }

            var subtotal = cart.Items.Sum(i => (i.Artwork?.Price ?? 0) * i.Quantity);
            var tax = subtotal * 0.18m;
            var total = subtotal + tax;

            return Json(new
            {
                success = true,
                subtotal = subtotal.ToString("N0"),
                tax = tax.ToString("N0"),
                total = total.ToString("N0")
            });
        }
    }
} 