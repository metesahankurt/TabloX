using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TabloX2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ArtworksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ArtworksController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var artworks = await _context.Artworks.Include(a => a.Artist).Include(a => a.Category).ToListAsync();
            return View(artworks);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Artists = new SelectList(await _context.Artists.ToListAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Artwork artwork)
        {
            if (ModelState.IsValid)
            {
                _context.Artworks.Add(artwork);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Artists = new SelectList(await _context.Artists.ToListAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(artwork);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork != null)
            {
                _context.Artworks.Remove(artwork);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null) return NotFound();
            ViewBag.Artists = new SelectList(await _context.Artists.ToListAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(artwork);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Artwork artwork)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"{key}: {error.ErrorMessage}");
                    }
                }
            }
            if (ModelState.IsValid)
            {
                _context.Artworks.Update(artwork);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Eser başarıyla güncellendi!";
                return RedirectToAction(nameof(Edit), new { id = artwork.Id });
            }
            TempData["Error"] = "Eser güncellenemedi. Lütfen bilgileri kontrol edin.";
            ViewBag.Artists = new SelectList(await _context.Artists.ToListAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(artwork);
        }
    }
} 