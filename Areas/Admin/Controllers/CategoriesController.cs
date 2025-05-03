using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;

namespace TabloX2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Artworks)
                .ToListAsync();
            return View(categories);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Artworks = await _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Category)
                .Where(a => a.CategoryId == null)
                .ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Category category, int[] SelectedArtworks)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();

                if (SelectedArtworks != null && SelectedArtworks.Length > 0)
                {
                    var artworks = await _context.Artworks
                        .Where(a => SelectedArtworks.Contains(a.Id))
                        .ToListAsync();

                    foreach (var artwork in artworks)
                    {
                        artwork.CategoryId = category.Id;
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Artworks)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            // Kategoriye ait olan ve olmayan tüm eserleri al
            ViewBag.AllArtworks = await _context.Artworks
                .Include(a => a.Artist)
                .ToListAsync();

            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category, int[] SelectedArtworks)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Önce mevcut kategoriyi güncelle
                    _context.Update(category);
                    await _context.SaveChangesAsync();

                    // Önce bu kategorideki tüm eserlerin kategori bağlantısını kaldır
                    var existingArtworks = await _context.Artworks
                        .Where(a => a.CategoryId == category.Id)
                        .ToListAsync();

                    foreach (var artwork in existingArtworks)
                    {
                        artwork.CategoryId = null;
                    }

                    // Sonra seçilen eserlerin kategori bağlantısını güncelle
                    if (SelectedArtworks != null && SelectedArtworks.Length > 0)
                    {
                        var selectedArtworks = await _context.Artworks
                            .Where(a => SelectedArtworks.Contains(a.Id))
                            .ToListAsync();

                        foreach (var artwork in selectedArtworks)
                        {
                            artwork.CategoryId = category.Id;
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(category);
        }
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
} 