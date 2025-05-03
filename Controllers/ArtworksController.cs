using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace TabloX2.Controllers
{
    public class ArtworksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ArtworksController> _logger;

        public ArtworksController(ApplicationDbContext context, ILogger<ArtworksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? categoryId = null, int? artistId = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Category)
                .AsQueryable();

            // Kategori filtresi
            if (categoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == categoryId);
            }

            // Sanatçı filtresi
            if (artistId.HasValue)
            {
                query = query.Where(a => a.ArtistId == artistId);
            }

            // Fiyat aralığı filtresi
            if (minPrice.HasValue)
            {
                query = query.Where(a => a.Price >= minPrice);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(a => a.Price <= maxPrice);
            }

            // Kategorileri ve sanatçıları ViewBag'e ekle
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Artists = await _context.Artists.ToListAsync();
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedArtistId = artistId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            var artworks = await query.OrderByDescending(a => a.Id).ToListAsync();
            return View(artworks);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var artwork = await _context.Artworks.Include(a => a.Category).Include(a => a.Artist).FirstOrDefaultAsync(a => a.Id == id);
            if (artwork == null) return NotFound();
            return View(artwork);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Categories, "Id", "Name");
            ViewBag.Artists = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Artists, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Artwork artwork)
        {
            if (ModelState.IsValid)
            {
                _context.Add(artwork);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
            ViewBag.Artists = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Artists, "Id", "Name", artwork.ArtistId);
            return View(artwork);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null) return NotFound();
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
            ViewBag.Artists = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Artists, "Id", "Name", artwork.ArtistId);
            return View(artwork);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Artwork artwork)
        {
            if (id != artwork.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(artwork);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
            ViewBag.Artists = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Artists, "Id", "Name", artwork.ArtistId);
            return View(artwork);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var artwork = await _context.Artworks.Include(a => a.Category).Include(a => a.Artist).FirstOrDefaultAsync(a => a.Id == id);
            if (artwork == null) return NotFound();
            return View(artwork);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);
            _context.Artworks.Remove(artwork);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
} 