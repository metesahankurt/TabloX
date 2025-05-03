using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TabloX2.Models;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace TabloX2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly List<HomeMessage> _messages = new List<HomeMessage>
    {
        new HomeMessage { Id = 1, Message = "Benzersiz sanat eserlerini keşfedin ve koleksiyonunuza ekleyin. Sanatın büyüleyici dünyasında size rehberlik etmekten mutluluk duyuyoruz." },
        new HomeMessage { Id = 2, Message = "Van Gogh'un yıldızlı gecelerinden Picasso'nun kübist eserlerine, sanat tarihinin en değerli hazinelerini keşfedin." },
        new HomeMessage { Id = 3, Message = "Her eserin arkasında bir hikaye var. Sanatçıların dünyasını keşfedin ve koleksiyonunuzu oluşturun." },
        new HomeMessage { Id = 4, Message = "Rönesans'tan modern sanata, farklı dönemlerin en çarpıcı eserlerini keşfedin." },
        new HomeMessage { Id = 5, Message = "Sanat, duyguların en güzel ifadesidir. Siz de bu dünyaya adım atın." },
        new HomeMessage { Id = 6, Message = "Her eser, sanatçının ruhundan bir parça taşır. Bu parçaları keşfetmeye hazır mısınız?" },
        new HomeMessage { Id = 7, Message = "Sanatın büyüleyici dünyasında, her eser size farklı bir pencere açar." },
        new HomeMessage { Id = 8, Message = "Dünyaca ünlü sanatçıların eserlerini keşfedin ve kendi sanat koleksiyonunuzu oluşturun." },
        new HomeMessage { Id = 9, Message = "Sanat, zamanın ötesinde bir yolculuktur. Bu yolculuğa bizimle çıkın." },
        new HomeMessage { Id = 10, Message = "Her eser, sanat tarihinin bir parçasıdır. Bu parçaları keşfetmeye hazır mısınız?" }
    };

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

        var random = new Random();
        var randomMessage = _messages[random.Next(_messages.Count)];
        ViewBag.HomeMessage = randomMessage.Message;

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
