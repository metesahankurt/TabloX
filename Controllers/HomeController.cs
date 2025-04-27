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

    public async Task<IActionResult> Index()
    {
        var allArtworks = await _context.Artworks.Include(a => a.Artist).Include(a => a.Category).OrderByDescending(a => a.Id).ToListAsync();
        return View(allArtworks);
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
