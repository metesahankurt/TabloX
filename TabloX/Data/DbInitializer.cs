using Microsoft.AspNetCore.Identity;
using TabloX.Models;

namespace TabloX.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Roller yoksa oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Artist"))
            {
                await roleManager.CreateAsync(new IdentityRole("Artist"));
            }

            // Admin kullanıcısı yoksa oluştur
            var adminUser = await userManager.FindByEmailAsync("admin@tablox.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@tablox.com",
                    Email = "admin@tablox.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Örnek sanatçı kullanıcısı oluştur
            var artistUser = await userManager.FindByEmailAsync("artist@tablox.com");
            if (artistUser == null)
            {
                artistUser = new ApplicationUser
                {
                    UserName = "artist@tablox.com",
                    Email = "artist@tablox.com",
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true,
                    IsArtist = true
                };
                await userManager.CreateAsync(artistUser, "Artist123!");
                await userManager.AddToRoleAsync(artistUser, "Artist");
            }

            // Kategoriler yoksa oluştur
            if (!context.Categories.Any())
            {
                var categories = new Category[]
                {
                    new Category { Name = "Yağlı Boya", Description = "Yağlı boya tablolar" },
                    new Category { Name = "Suluboya", Description = "Suluboya tablolar" },
                    new Category { Name = "Dijital Sanat", Description = "Dijital ortamda oluşturulan sanat eserleri" },
                    new Category { Name = "Fotoğraf", Description = "Sanatsal fotoğraflar" },
                    new Category { Name = "Heykel", Description = "Üç boyutlu sanat eserleri" }
                };

                foreach (var category in categories)
                {
                    context.Categories.Add(category);
                }
                await context.SaveChangesAsync();
            }

            // Örnek ürünler yoksa oluştur
            if (!context.Products.Any())
            {
                var products = new Product[]
                {
                    new Product {
                        Title = "Gün Batımı Manzarası",
                        Description = "Deniz kenarında muhteşem bir gün batımı manzarası. Tuval üzerine yağlı boya tekniği ile yapılmış özgün eser.",
                        Price = 1500.00M,
                        ImageUrl = "https://images.unsplash.com/photo-1518998053901-5348d3961a04?w=800&auto=format&fit=crop",
                        CreatedAt = DateTime.Now,
                        CategoryId = 1,
                        ArtistId = artistUser.Id
                    },
                    new Product {
                        Title = "Soyut Kompozisyon",
                        Description = "Modern sanatın en güzel örneklerinden biri. Canlı renkler ve dinamik fırça darbeleriyle oluşturulmuş soyut kompozisyon.",
                        Price = 2000.00M,
                        ImageUrl = "https://images.unsplash.com/photo-1541961017774-22349e4a1262?w=800&auto=format&fit=crop",
                        CreatedAt = DateTime.Now.AddDays(-1),
                        CategoryId = 1,
                        ArtistId = artistUser.Id
                    },
                    new Product {
                        Title = "Sisli Orman",
                        Description = "Sisli bir orman yolunun suluboya çalışması. Doğanın gizemli atmosferini yansıtan etkileyici bir eser.",
                        Price = 1200.00M,
                        ImageUrl = "https://images.unsplash.com/photo-1598439210625-5067c578f3f6?w=800&auto=format&fit=crop",
                        CreatedAt = DateTime.Now.AddDays(-2),
                        CategoryId = 2,
                        ArtistId = artistUser.Id
                    },
                    new Product {
                        Title = "Modern Portre",
                        Description = "Dijital tekniklerle oluşturulmuş modern bir portre çalışması. Çağdaş sanatın yenilikçi bir örneği.",
                        Price = 800.00M,
                        ImageUrl = "https://images.unsplash.com/photo-1500462918059-b1a0cb512f1d?w=800&auto=format&fit=crop",
                        CreatedAt = DateTime.Now.AddDays(-3),
                        CategoryId = 3,
                        ArtistId = artistUser.Id
                    },
                    new Product {
                        Title = "Şehir Işıkları",
                        Description = "Gece şehir manzarası. Uzun pozlama tekniği ile çekilmiş etkileyici bir fotoğraf.",
                        Price = 950.00M,
                        ImageUrl = "https://images.unsplash.com/photo-1519608425089-7f4bdf8829bb?w=800&auto=format&fit=crop",
                        CreatedAt = DateTime.Now.AddDays(-4),
                        CategoryId = 4,
                        ArtistId = artistUser.Id
                    },
                    new Product {
                        Title = "Bronz Heykel",
                        Description = "El yapımı bronz heykel. Modern sanatın üç boyutlu bir yorumu.",
                        Price = 2500.00M,
                        ImageUrl = "https://images.unsplash.com/photo-1544531585-9847b68c8c86?w=800&auto=format&fit=crop",
                        CreatedAt = DateTime.Now.AddDays(-5),
                        CategoryId = 5,
                        ArtistId = artistUser.Id
                    }
                };

                foreach (var product in products)
                {
                    context.Products.Add(product);
                }
                await context.SaveChangesAsync();
            }
        }
    }
} 