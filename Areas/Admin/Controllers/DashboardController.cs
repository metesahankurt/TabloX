using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using System.Threading.Tasks;

namespace TabloX2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // İstatistikleri hesapla
            ViewBag.CategoryCount = await _context.Categories.CountAsync();
            ViewBag.ArtistCount = await _context.Artists.CountAsync();
            ViewBag.ArtworkCount = await _context.Artworks.CountAsync();
            ViewBag.UserCount = await _userManager.Users.CountAsync();

            return View();
        }
    }
} 