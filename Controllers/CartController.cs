using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TabloX2.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sepeti görüntüle
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _context.CartItems.Include(c => c.Artwork).ThenInclude(a => a.Artist).Where(c => c.UserId == userId).ToListAsync();
            return View(cartItems);
        }

        // Sepete ekle
        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existing = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ArtworkId == id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                var artwork = await _context.Artworks.FindAsync(id);
                if (artwork == null)
                {
                    return NotFound();
                }
                _context.CartItems.Add(new CartItem 
                { 
                    UserId = userId, 
                    ArtworkId = id, 
                    Artwork = artwork,
                    Quantity = 1 
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Sepetten çıkar
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Increase(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id);
            if (item != null)
            {
                item.Quantity++;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Decrease(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id);
            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
} 