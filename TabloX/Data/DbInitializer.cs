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

            if (!context.Users.Any())
            {
                // Sanatçı kullanıcıları oluştur
                var artists = new List<(string Email, string FirstName, string LastName)>
                {
                    ("vangogh@example.com", "Vincent", "van Gogh"),
                    ("monet@example.com", "Claude", "Monet"),
                    ("davinci@example.com", "Leonardo", "da Vinci"),
                    ("picasso@example.com", "Pablo", "Picasso"),
                    ("dali@example.com", "Salvador", "Dali"),
                    ("klimt@example.com", "Gustav", "Klimt"),
                    ("munch@example.com", "Edvard", "Munch"),
                    ("kahlo@example.com", "Frida", "Kahlo"),
                    ("rembrandt@example.com", "Rembrandt", "van Rijn"),
                    ("vermeer@example.com", "Johannes", "Vermeer")
                };

                foreach (var artist in artists)
                {
                    var user = new ApplicationUser
                    {
                        UserName = artist.Email,
                        Email = artist.Email,
                        FirstName = artist.FirstName,
                        LastName = artist.LastName,
                        EmailConfirmed = true,
                        IsArtist = true
                    };
                    await userManager.CreateAsync(user, "Artist123!");
                    await userManager.AddToRoleAsync(user, "Artist");
                }
            }

            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Yağlı Boya", Description = "Yağlı boya tekniği ile yapılmış eserler" },
                    new Category { Name = "Soyut", Description = "Soyut sanat eserleri" },
                    new Category { Name = "Modern", Description = "Modern sanat eserleri" },
                    new Category { Name = "Klasik", Description = "Klasik dönem eserleri" },
                    new Category { Name = "Empresyonist", Description = "Empresyonist akım eserleri" },
                    new Category { Name = "Portre", Description = "Portre çalışmaları" },
                    new Category { Name = "Manzara", Description = "Manzara resimleri" },
                    new Category { Name = "Natürmort", Description = "Natürmort çalışmaları" }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Title = "Yıldızlı Gece",
                        Description = "Van Gogh'un en ünlü eserlerinden biri olan Yıldızlı Gece, sanatçının kendine has üslubuyla gökyüzünü tasvir ettiği başyapıtıdır.",
                        Price = 2500.00M,
                        ImageUrl = "/images/artworks/starry_night.jpg",
                        CategoryId = context.Categories.First(c => c.Name == "Yağlı Boya").Id,
                        ArtistId = context.Users.First(u => u.Email == "vangogh@example.com").Id
                    },
                    new Product
                    {
                        Title = "Mona Lisa",
                        Description = "Leonardo da Vinci'nin dünyaca ünlü portre çalışması.",
                        Price = 3000.00M,
                        ImageUrl = "/images/artworks/mona_lisa.jpg",
                        CategoryId = context.Categories.First(c => c.Name == "Portre").Id,
                        ArtistId = context.Users.First(u => u.Email == "davinci@example.com").Id
                    },
                    new Product
                    {
                        Title = "Nilüferler",
                        Description = "Monet'nin ünlü bahçesindeki nilüferleri resmettiği empresyonist başyapıt.",
                        Price = 2000.00M,
                        ImageUrl = "/images/artworks/water_lilies.jpg",
                        CategoryId = context.Categories.First(c => c.Name == "Empresyonist").Id,
                        ArtistId = context.Users.First(u => u.Email == "monet@example.com").Id
                    },
                    new Product
                    {
                        Title = "Guernica",
                        Description = "Picasso'nun savaş karşıtı en önemli eserlerinden biri.",
                        Price = 2800.00M,
                        ImageUrl = "/images/artworks/guernica.jpg",
                        CategoryId = context.Categories.First(c => c.Name == "Modern").Id,
                        ArtistId = context.Users.First(u => u.Email == "picasso@example.com").Id
                    },
                    new Product
                    {
                        Title = "Belleğin Azmi",
                        Description = "Salvador Dali'nin eriyen saatleri resmettiği sürrealist başyapıtı.",
                        Price = 2300.00M,
                        ImageUrl = "/images/artworks/persistence_of_memory.jpg",
                        CategoryId = context.Categories.First(c => c.Name == "Soyut").Id,
                        ArtistId = context.Users.First(u => u.Email == "dali@example.com").Id
                    },
                    new Product
                    {
                        Title = "Öpücük",
                        Description = "Gustav Klimt'in altın varaklı romantik başyapıtı.",
                        Price = 2600.00M,
                        ImageUrl = "/images/artworks/the_kiss.jpg",
                        CategoryId = context.Categories.First(c => c.Name == "Klasik").Id,
                        ArtistId = context.Users.First(u => u.Email == "klimt@example.com").Id
                    },
                    new Product
                    {
                        Title = "Çığlık",
                        Description = "Edvard Munch'un modern sanatın sembolü haline gelen eseri.",
                        Price = 2400.00M,
                        ImageUrl = "/images/artworks/the_scream.jpg",
                        CategoryId = context.Categories.First(c => c.Name == "Modern").Id,
                        ArtistId = context.Users.First(u => u.Email == "munch@example.com").Id
                    },
                    new Product
                    {
                        Title = "Otoportre",
                        Description = "Frida Kahlo'nun kendini resmettiği ünlü otoportresi.",
                        Price = 2100.00M,
                        ImageUrl = "/images/artworks/self_portrait.jpg",
                        CategoryId = context.Categories.First(c => c.Name == "Portre").Id,
                        ArtistId = context.Users.First(u => u.Email == "kahlo@example.com").Id
                    },
                    new Product
                    {
                        Title = "Gece Nöbeti",
                        Description = "Rembrandt'ın ışık-gölge ustası olduğunu gösteren başyapıtı.",
                        Price = 2700.00M,
                        ImageUrl = "/images/artworks/night_watch.jpg",
                        CategoryId = context.Categories.First(c => c.Name == "Klasik").Id,
                        ArtistId = context.Users.First(u => u.Email == "rembrandt@example.com").Id
                    },
                    new Product
                    {
                        Title = "İnci Küpeli Kız",
                        Description = "Vermeer'in en ünlü portre çalışması.",
                        Price = 2200.00M,
                        ImageUrl = "/images/artworks/girl_with_pearl_earring.jpg",
                        CategoryId = context.Categories.First(c => c.Name == "Portre").Id,
                        ArtistId = context.Users.First(u => u.Email == "vermeer@example.com").Id
                    }
                };

                // Daha fazla ürün ekle
                var random = new Random();
                var artists = context.Users.ToList();
                var categories = context.Categories.ToList();

                var additionalTitles = new[]
                {
                    "Sonbahar Manzarası", "Deniz Kenarında", "Soyut Kompozisyon",
                    "Çiçek Bahçesi", "Dağ Manzarası", "Şehir Işıkları",
                    "Orman Yolu", "Güneş Batımı", "Köy Hayatı",
                    "Nehir Kıyısı", "Eski Köprü", "Modern Şehir",
                    "Balıkçı Tekneleri", "Yağmurlu Gün", "Kış Manzarası"
                };

                foreach (var title in additionalTitles)
                {
                    var artist = artists[random.Next(artists.Count)];
                    var category = categories[random.Next(categories.Count)];
                    var price = random.Next(1000, 3000);

                    products.Add(new Product
                    {
                        Title = title,
                        Description = $"{artist.FirstName} {artist.LastName}'in {category.Name.ToLower()} tarzında bir eseri.",
                        Price = price,
                        ImageUrl = $"/images/artworks/{title.ToLower().Replace(" ", "_")}.jpg",
                        CategoryId = category.Id,
                        ArtistId = artist.Id
                    });
                }

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
} 