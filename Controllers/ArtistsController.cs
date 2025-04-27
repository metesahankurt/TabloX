using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using System.Threading.Tasks;

namespace TabloX2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArtistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ArtistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Artists.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var artist = await _context.Artists.Include(a => a.Artworks).ThenInclude(e => e.Category).FirstOrDefaultAsync(a => a.Id == id);
            if (artist == null) return NotFound();
            return View(artist);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Artist artist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return NotFound();
            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Artist artist)
        {
            if (id != artist.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return NotFound();
            return View(artist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
} 