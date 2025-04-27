using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using System.Threading.Tasks;
using System.Linq;

namespace TabloX2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArtworksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ArtworksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var artworks = _context.Artworks.Include(a => a.Category).Include(a => a.Artist);
            return View(await artworks.ToListAsync());
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null) return NotFound();
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
            ViewBag.Artists = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Artists, "Id", "Name", artwork.ArtistId);
            return View(artwork);
        }

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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var artwork = await _context.Artworks.Include(a => a.Category).Include(a => a.Artist).FirstOrDefaultAsync(a => a.Id == id);
            if (artwork == null) return NotFound();
            return View(artwork);
        }

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