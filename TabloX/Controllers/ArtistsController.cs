using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX.Data;
using TabloX.Models;

namespace TabloX.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ArtistsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var artistProfiles = await _context.ArtistProfiles
                .Include(ap => ap.User)
                .ToListAsync();
            return View(artistProfiles);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artistProfile = await _context.ArtistProfiles
                .Include(ap => ap.User)
                .ThenInclude(u => u.Products)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(ap => ap.UserId == id);

            if (artistProfile == null)
            {
                return NotFound();
            }

            return View(artistProfile);
        }

        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var profile = await _context.ArtistProfiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> Edit([Bind("Id,UserId,Bio,Website,Instagram,Facebook")] ArtistProfile profile)
        {
            if (!ModelState.IsValid)
            {
                return View(profile);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (profile.UserId != user.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(profile);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistProfileExists(profile.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Details), new { id = profile.UserId });
        }

        private bool ArtistProfileExists(int id)
        {
            return _context.ArtistProfiles.Any(e => e.Id == id);
        }
    }
} 