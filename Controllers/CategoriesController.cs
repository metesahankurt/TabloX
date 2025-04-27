using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TabloX2.Data;

namespace ArtGallery.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Herkese açık liste
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // Herkese açık detay
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories.Include(c => c.Artworks).ThenInclude(e => e.Artist).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        // Yönetim aksiyonları (Create, Edit, Delete) sadece admin
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
    }
} 