using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX.Data;
using TabloX.Models;
using System.IO;
using System.Text;

namespace TabloX.Controllers
{
    public class SeedController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SeedController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> HardCleanup()
        {
            try
            {
                // Önce tüm ürünleri silelim
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Products");
                
                // Şimdi yeni ürünleri ekleyelim
                var artworksPath = Path.Combine(_environment.WebRootPath, "images", "artworks");
                var imageFiles = Directory.GetFiles(artworksPath, "*.jpg");
                var artists = await _context.Users.Where(u => u.IsArtist).ToListAsync();
                var categories = await _context.Categories.ToListAsync();
                var random = new Random();

                foreach (var imageFile in imageFiles.Take(20)) // İlk 20 görseli kullanalım
                {
                    var fileName = Path.GetFileName(imageFile);
                    var artist = artists[random.Next(artists.Count)];
                    var category = categories[random.Next(categories.Count)];

                    var product = new Product
                    {
                        Title = GetTitleFromFileName(fileName),
                        Description = $"Sanatçı {artist.FullName} tarafından yapılmış {category.Name} tarzında bir eser.",
                        Price = random.Next(1500, 3500),
                        ImageUrl = $"/images/artworks/{fileName}",
                        CategoryId = category.Id,
                        ArtistId = artist.Id,
                        CreatedAt = DateTime.Now
                    };

                    _context.Products.Add(product);
                }

                await _context.SaveChangesAsync();
                return Content("Veritabanı temizlendi ve yeni ürünler eklendi.");
            }
            catch (Exception ex)
            {
                return Content($"Hata oluştu: {ex.Message}");
            }
        }

        public async Task<IActionResult> VerifyCleanup()
        {
            var products = await _context.Products.ToListAsync();
            var sb = new StringBuilder();
            sb.AppendLine("Mevcut ürünler:");
            foreach (var product in products)
            {
                sb.AppendLine($"- {product.Title}");
            }
            return Content(sb.ToString());
        }

        public async Task<IActionResult> CleanupProducts()
        {
            var artworksPath = Path.Combine(_environment.WebRootPath, "images", "artworks");
            var existingImageFiles = Directory.GetFiles(artworksPath, "*.jpg")
                                           .Select(f => Path.GetFileName(f))
                                           .ToHashSet();

            var products = await _context.Products.ToListAsync();
            var productsToRemove = new List<Product>();

            foreach (var product in products)
            {
                var imageFileName = Path.GetFileName(product.ImageUrl.Replace("/images/artworks/", ""));
                if (!existingImageFiles.Contains(imageFileName))
                {
                    productsToRemove.Add(product);
                }
            }

            if (productsToRemove.Any())
            {
                _context.Products.RemoveRange(productsToRemove);
                await _context.SaveChangesAsync();
                return Content($"{productsToRemove.Count} adet görüntüsü eksik ürün kaldırıldı.");
            }

            return Content("Tüm ürünlerin görselleri mevcut.");
        }

        public async Task<IActionResult> SeedProducts()
        {
            var artworksPath = Path.Combine(_environment.WebRootPath, "images", "artworks");
            var imageFiles = Directory.GetFiles(artworksPath, "*.jpg")
                                    .Where(f => !_context.Products.Any(p => p.ImageUrl.Contains(Path.GetFileName(f))))
                                    .ToArray();

            if (imageFiles.Length < 2)
            {
                return Content("Yeterli sayıda kullanılmamış görsel bulunamadı.");
            }

            var artists = await _context.Users.Where(u => u.IsArtist).ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            var random = new Random();
            var products = new List<Product>();

            for (int i = 0; i < 2; i++)
            {
                var imageFile = imageFiles[random.Next(imageFiles.Length)];
                var fileName = Path.GetFileName(imageFile);
                var artist = artists[random.Next(artists.Count)];
                var category = categories[random.Next(categories.Count)];

                var product = new Product
                {
                    Title = GetTitleFromFileName(fileName),
                    Description = $"Sanatçı {artist.FullName} tarafından yapılmış {category.Name} tarzında bir eser.",
                    Price = random.Next(1500, 3500),
                    ImageUrl = $"/images/artworks/{fileName}",
                    CategoryId = category.Id,
                    ArtistId = artist.Id,
                    CreatedAt = DateTime.Now
                };

                products.Add(product);
            }

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> RemoveSpecificProducts()
        {
            var titlesToRemove = new[]
            {
                "The Theater Box",
                "The Ballet Class",
                "L'Absinthe",
                "Race Horses",
                "Still Life with Apples"
            };

            var productsToRemove = await _context.Products
                .Where(p => titlesToRemove.Contains(p.Title))
                .ToListAsync();

            if (productsToRemove.Any())
            {
                _context.Products.RemoveRange(productsToRemove);
                await _context.SaveChangesAsync();
                return Content($"{productsToRemove.Count} adet ürün başarıyla kaldırıldı.");
            }

            return Content("Kaldırılacak ürün bulunamadı.");
        }

        public async Task<IActionResult> ListAllProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Artist)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Mevcut Ürünler:");
            foreach (var product in products)
            {
                sb.AppendLine($"- {product.Title} (ID: {product.Id})");
                sb.AppendLine($"  Görsel: {product.ImageUrl}");
                sb.AppendLine($"  Kategori: {product.Category?.Name}");
                sb.AppendLine($"  Sanatçı: {product.Artist?.FullName}");
                sb.AppendLine();
            }

            return Content(sb.ToString());
        }

        public async Task<IActionResult> ForceRemoveProducts()
        {
            var titlesToRemove = new[]
            {
                "The Theater Box",
                "The Ballet Class",
                "L'Absinthe",
                "Race Horses",
                "Still Life with Apples",
                "Wheat Field with Cypresses"
            };

            // Doğrudan SQL sorgusu ile silme işlemi
            foreach (var title in titlesToRemove)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM Products WHERE Title = {0}", title);
            }

            await _context.SaveChangesAsync();
            return Content("Ürünler zorla kaldırıldı.");
        }

        public async Task<IActionResult> ResetProducts()
        {
            try
            {
                // Tüm ürünleri sil
                _context.Products.RemoveRange(_context.Products);
                await _context.SaveChangesAsync();

                // Mevcut görselleri kontrol et
                var artworksPath = Path.Combine(_environment.WebRootPath, "images", "artworks");
                var imageFiles = Directory.GetFiles(artworksPath, "*.jpg")
                    .Where(f => !f.Contains("theater") && 
                           !f.Contains("ballet") && 
                           !f.Contains("absinthe") && 
                           !f.Contains("race") && 
                           !f.Contains("still_life") &&
                           !f.Contains("wheat"))
                    .Take(20);

                var artists = await _context.Users.Where(u => u.IsArtist).ToListAsync();
                var categories = await _context.Categories.ToListAsync();
                var random = new Random();

                // Yeni ürünleri ekle
                foreach (var imageFile in imageFiles)
                {
                    var fileName = Path.GetFileName(imageFile);
                    var artist = artists[random.Next(artists.Count)];
                    var category = categories[random.Next(categories.Count)];

                    var product = new Product
                    {
                        Title = GetTitleFromFileName(fileName),
                        Description = $"Sanatçı {artist.FullName} tarafından yapılmış {category.Name} tarzında bir eser.",
                        Price = random.Next(1500, 3500),
                        ImageUrl = $"/images/artworks/{fileName}",
                        CategoryId = category.Id,
                        ArtistId = artist.Id,
                        CreatedAt = DateTime.Now
                    };

                    _context.Products.Add(product);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Content($"Hata oluştu: {ex.Message}");
            }
        }

        private string GetTitleFromFileName(string fileName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            name = name.Replace('_', ' ');
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
        }
    }
} 