using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace TabloX2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Kategorileri yükle
            ViewBag.Categories = await _context.Categories.ToListAsync();

            // Sanatçıları yükle
            ViewBag.Artists = await _context.Artists.ToListAsync();

            // Eserleri yükle (sanatçı ve kategori bilgileriyle birlikte)
            ViewBag.Artworks = await _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Category)
                .ToListAsync();

            // Siparişleri yükle (sipariş detaylarıyla birlikte)
            ViewBag.Orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Artwork)
                .ToListAsync();

            // Kullanıcıları yükle
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<object>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }
            
            ViewBag.Users = userList;

            var model = new AdminDashboardViewModel
            {
                RecentOrders = _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Artwork)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList() ?? new List<Order>(),

                TopSellingArtworks = _context.Artworks
                    .Include(a => a.Artist)
                    .OrderByDescending(a => a.SalesCount)
                    .Take(5)
                    .Select(a => new TopSellingArtwork { Artwork = a, TotalSales = a.SalesCount })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CleanupDatabase()
        {
            // Eseri olmayan sanatçıları bul
            var artistsWithoutArtworks = await _context.Artists
                .Where(a => !a.Artworks.Any())
                .ToListAsync();

            // Eseri olmayan kategorileri bul
            var categoriesWithoutArtworks = await _context.Categories
                .Where(c => !c.Artworks.Any())
                .ToListAsync();

            // Eseri olmayan sanatçıları sil
            _context.Artists.RemoveRange(artistsWithoutArtworks);

            // Eseri olmayan kategorileri sil
            _context.Categories.RemoveRange(categoriesWithoutArtworks);

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();

            // Sonuçları görüntüle
            return Json(new
            {
                RemovedArtists = artistsWithoutArtworks.Select(a => a.Name).ToList(),
                RemovedCategories = categoriesWithoutArtworks.Select(c => c.Name).ToList(),
                RemainingArtists = await _context.Artists
                    .Select(a => new { a.Name, ArtworkCount = a.Artworks.Count })
                    .ToListAsync(),
                RemainingCategories = await _context.Categories
                    .Select(c => new { c.Name, ArtworkCount = c.Artworks.Count })
                    .ToListAsync()
            });
        }

        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalArtworks = await _context.Artworks.CountAsync(),
                TotalArtists = await _context.Artists.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                RecentOrders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Artwork)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync(),
                TopSellingArtworks = await _context.OrderItems
                    .Include(oi => oi.Artwork)
                    .GroupBy(oi => oi.ArtworkId)
                    .Select(g => new TopSellingArtwork
                    {
                        Artwork = g.First().Artwork,
                        TotalSales = g.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(x => x.TotalSales)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        public IActionResult ArtworkDetails(int id)
        {
            var artwork = _context.Artworks
                .Include(a => a.Artist)
                .FirstOrDefault(a => a.Id == id);

            if (artwork == null)
            {
                return NotFound();
            }

            return View(artwork);
        }
    }

    public class AdminDashboardViewModel
    {
        public int TotalArtworks { get; set; }
        public int TotalArtists { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public required List<Order> RecentOrders { get; set; }
        public required List<TopSellingArtwork> TopSellingArtworks { get; set; }
    }

    public class TopSellingArtwork
    {
        public required Artwork Artwork { get; set; }
        public int TotalSales { get; set; }
    }
} 