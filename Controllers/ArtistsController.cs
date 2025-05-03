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
    public class ArtistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ArtistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Artists.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(a => 
                    a.Name.ToLower().Contains(search) || 
                    a.Surname.ToLower().Contains(search));
            }

            var artists = await query.ToListAsync();
            ViewBag.Search = search;
            
            return View(artists);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            var artists = await _context.Artists
                .Where(a => a.Name.Contains(term))
                .Select(a => new { 
                    id = a.Id,
                    name = a.Name,
                    initials = $"{a.Name[0]}{a.Surname[0]}",
                    bio = a.Bio
                })
                .Take(5)
                .ToListAsync();
            
            return Json(artists);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .Include(a => a.Artworks)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Surname,Bio,Description,Country")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Bio,Description,Country")] Artist artist)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .Include(a => a.Artworks)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }
    }
} 