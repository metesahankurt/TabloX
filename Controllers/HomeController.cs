using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TabloX2.Models;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TabloX2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(int? categoryId = null, int? artistId = null, string priceRange = null)
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
        if (!string.IsNullOrEmpty(priceRange))
        {
            var prices = priceRange.Split('-');
            if (prices.Length == 2 && decimal.TryParse(prices[0], out decimal minPrice) && decimal.TryParse(prices[1], out decimal maxPrice))
            {
                query = query.Where(a => a.Price >= minPrice && a.Price <= maxPrice);
            }
        }

        var artworks = await query.OrderByDescending(a => a.Id).ToListAsync();
        
        // ViewBag'e gerekli verileri ekle
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Artists = await _context.Artists.ToListAsync();
        ViewBag.SelectedCategoryId = categoryId;
        ViewBag.SelectedArtistId = artistId;
        ViewBag.SelectedPriceRange = priceRange;

        return View(artworks);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
