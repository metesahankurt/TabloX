using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TabloX2.Models;
using System.Text.Json.Serialization;
using System.IO;
using System.Net;

namespace TabloX2.Data
{
    public class MetMuseumSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            try
            {
                // Önce kategorileri kontrol et
                if (!context.Categories.Any())
                {
                    CategorySeeder.Seed(context);
                }

                // Tüm sanatçıları ve eserleri sil
                context.Artworks.RemoveRange(context.Artworks);
                context.Artists.RemoveRange(context.Artists);
                context.SaveChanges();

                // Kategorileri önceden yükle
                var categories = context.Categories.ToDictionary(c => c.Name, c => c);
                var defaultCategory = categories["Modern Yaşam"];

                // Sanatçı listesi (isim ve soyisim ayrı)
                var allowedArtists = new Dictionary<string, (string Name, string Surname)> {
                    { "Vincent van Gogh", ("Vincent", "van Gogh") },
                    { "Claude Monet", ("Claude", "Monet") },
                    { "Leonardo da Vinci", ("Leonardo", "da Vinci") },
                    { "Pablo Picasso", ("Pablo", "Picasso") },
                    { "Rembrandt", ("Rembrandt", "van Rijn") },
                    { "Johannes Vermeer", ("Johannes", "Vermeer") },
                    { "Michelangelo Buonarroti", ("Michelangelo", "Buonarroti") },
                    { "Gustav Klimt", ("Gustav", "Klimt") },
                    { "Salvador Dalí", ("Salvador", "Dalí") },
                    { "Frida Kahlo", ("Frida", "Kahlo") },
                    { "Edvard Munch", ("Edvard", "Munch") },
                    { "Henri Matisse", ("Henri", "Matisse") },
                    { "Paul Gauguin", ("Paul", "Gauguin") },
                    { "Auguste Renoir", ("Auguste", "Renoir") },
                    { "Edgar Degas", ("Edgar", "Degas") },
                    { "Andy Warhol", ("Andy", "Warhol") },
                    { "Georgia O'Keeffe", ("Georgia", "O'Keeffe") },
                    { "Paul Cézanne", ("Paul", "Cézanne") },
                    { "Wassily Kandinsky", ("Wassily", "Kandinsky") },
                    { "Diego Velázquez", ("Diego", "Velázquez") }
                };

                // Sanatçı biyografileri
                var artistBios = new Dictionary<string, string> {
                    { "Vincent van Gogh", "Hollandalı post-empresyonist ressam. 1853-1890 yılları arasında yaşamış, duygusal yoğunluklu eserleriyle tanınır." },
                    { "Claude Monet", "Fransız empresyonist ressam. 1840-1926 yılları arasında yaşamış, doğa ve ışık üzerine çalışmalarıyla tanınır." },
                    { "Leonardo da Vinci", "İtalyan Rönesans dönemi sanatçısı, mucit ve bilim insanı. 1452-1519 yılları arasında yaşamıştır." },
                    { "Pablo Picasso", "İspanyol ressam ve heykeltıraş. 1881-1973 yılları arasında yaşamış, kübizmin öncüsüdür." },
                    { "Rembrandt", "Hollandalı Barok dönem ressamı. 1606-1669 yılları arasında yaşamış, ışık-gölge kullanımıyla ünlüdür." },
                    { "Johannes Vermeer", "Hollandalı Barok dönem ressamı. 1632-1675 yılları arasında yaşamış, günlük yaşam sahneleriyle ünlüdür." },
                    { "Michelangelo Buonarroti", "İtalyan Rönesans dönemi heykeltıraş, mimar ve ressam. 1475-1564 yılları arasında yaşamıştır." },
                    { "Gustav Klimt", "Avusturyalı sembolist ressam. 1862-1918 yılları arasında yaşamış, Art Nouveau akımının öncülerindendir." },
                    { "Salvador Dalí", "İspanyol sürrealist ressam. 1904-1989 yılları arasında yaşamış, fantastik eserleriyle tanınır." },
                    { "Frida Kahlo", "Meksikalı ressam. 1907-1954 yılları arasında yaşamış, otoportreleri ve Meksika kültürünü yansıtan eserleriyle ünlüdür." },
                    { "Edvard Munch", "Norveçli ekspresyonist ressam. 1863-1944 yılları arasında yaşamış, 'Çığlık' tablosuyla ünlüdür." },
                    { "Henri Matisse", "Fransız ressam. 1869-1954 yılları arasında yaşamış, fovizmin öncülerindendir." },
                    { "Paul Gauguin", "Fransız post-empresyonist ressam. 1848-1903 yılları arasında yaşamış, Tahiti resimleriyle tanınır." },
                    { "Auguste Renoir", "Fransız empresyonist ressam. 1841-1919 yılları arasında yaşamış, portre ve günlük yaşam sahneleriyle ünlüdür." },
                    { "Edgar Degas", "Fransız empresyonist ressam. 1834-1917 yılları arasında yaşamış, balerinleri ve hareketi betimleyen eserleriyle tanınır." },
                    { "Andy Warhol", "Amerikalı pop art sanatçısı. 1928-1987 yılları arasında yaşamış, popüler kültür ikonlarını resmeder." },
                    { "Georgia O'Keeffe", "Amerikalı modernist ressam. 1887-1986 yılları arasında yaşamış, çiçek resimleriyle tanınır." },
                    { "Paul Cézanne", "Fransız post-empresyonist ressam. 1839-1906 yılları arasında yaşamış, modern sanatın öncülerindendir." },
                    { "Wassily Kandinsky", "Rus ressam ve sanat teorisyeni. 1866-1944 yılları arasında yaşamış, soyut sanatın öncülerindendir." },
                    { "Diego Velázquez", "İspanyol Barok dönem ressamı. 1599-1660 yılları arasında yaşamış, portre ve tarihi sahneleriyle ünlüdür." }
                };

                // Sanatçı ülkeleri
                var artistCountries = new Dictionary<string, string> {
                    { "Vincent van Gogh", "Hollanda" },
                    { "Claude Monet", "Fransa" },
                    { "Leonardo da Vinci", "İtalya" },
                    { "Pablo Picasso", "İspanya" },
                    { "Rembrandt", "Hollanda" },
                    { "Johannes Vermeer", "Hollanda" },
                    { "Michelangelo Buonarroti", "İtalya" },
                    { "Gustav Klimt", "Avusturya" },
                    { "Salvador Dalí", "İspanya" },
                    { "Frida Kahlo", "Meksika" },
                    { "Edvard Munch", "Norveç" },
                    { "Henri Matisse", "Fransa" },
                    { "Paul Gauguin", "Fransa" },
                    { "Auguste Renoir", "Fransa" },
                    { "Edgar Degas", "Fransa" },
                    { "Andy Warhol", "Amerika Birleşik Devletleri" },
                    { "Georgia O'Keeffe", "Amerika Birleşik Devletleri" },
                    { "Paul Cézanne", "Fransa" },
                    { "Wassily Kandinsky", "Rusya" },
                    { "Diego Velázquez", "İspanya" }
                };

                // Önce sanatçıları ekle
                foreach (var artist in allowedArtists)
                {
                    if (context.Artists.Any(a => a.Name == artist.Value.Name && a.Surname == artist.Value.Surname)) continue;
                    
                    string artistBio = artistBios.ContainsKey(artist.Key) ? artistBios[artist.Key] : "";
                    string country = artistCountries.ContainsKey(artist.Key) ? artistCountries[artist.Key] : "Bilinmiyor";
                    
                    var artistEntity = new Artist { 
                        Name = artist.Value.Name,
                        Surname = artist.Value.Surname,
                        Bio = artistBio,
                        Description = artistBio,
                        Country = country
                    };
                    context.Artists.Add(artistEntity);
                }
                context.SaveChanges();

                // wwwroot/images klasörünü oluştur
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                using var httpClient = new HttpClient(handler);

                var artworks = new List<Artwork>();
                var addedTitles = new HashSet<string>();
                int added = 0;

                foreach (var artist in allowedArtists)
                {
                    if (added >= 48) break;

                    var searchUrl = $"https://collectionapi.metmuseum.org/public/collection/v1/search?hasImages=true&q={System.Web.HttpUtility.UrlEncode(artist.Key)}";
                    var searchResponse = await httpClient.GetStringAsync(searchUrl);
                    var searchObj = JsonSerializer.Deserialize<MetSearchResult>(searchResponse);
                    var objectIds = searchObj?.objectIDs?.Take(10).ToList() ?? new List<int>();

                    foreach (var id in objectIds)
                    {
                        if (added >= 48) break;

                        try
                        {
                            var objResponse = await httpClient.GetStringAsync($"https://collectionapi.metmuseum.org/public/collection/v1/objects/{id}");
                            var obj = JsonSerializer.Deserialize<MetObject>(objResponse);

                            if (obj == null || string.IsNullOrEmpty(obj.primaryImage) || string.IsNullOrEmpty(obj.title)) continue;
                            if (addedTitles.Contains(obj.title)) continue;
                            if (!string.IsNullOrEmpty(obj.classification) && !obj.classification.ToLower().Contains("painting")) continue;
                            if (obj.artistDisplayName != artist.Key) continue;

                            // Görselleri indir ve kaydet
                            string imageId = obj.primaryImage.Split('/').Last().Split('.').First();
                            string thumbnailUrl = $"https://images.metmuseum.org/CRDImages/ep/web-large/{imageId}.jpg";
                            string mediumUrl = $"https://images.metmuseum.org/CRDImages/ep/web-additional/{imageId}.jpg";
                            string highResUrl = $"https://images.metmuseum.org/CRDImages/ep/original/{imageId}.jpg";

                            // Görselleri indir
                            string thumbnailPath = Path.Combine(imagesPath, $"thumbnail_{imageId}.jpg");
                            string mediumPath = Path.Combine(imagesPath, $"medium_{imageId}.jpg");
                            string highResPath = Path.Combine(imagesPath, $"highres_{imageId}.jpg");

                            using (var response = await httpClient.GetAsync(thumbnailUrl))
                            using (var fileStream = new FileStream(thumbnailPath, FileMode.Create))
                            {
                                await response.Content.CopyToAsync(fileStream);
                            }

                            using (var response = await httpClient.GetAsync(mediumUrl))
                            using (var fileStream = new FileStream(mediumPath, FileMode.Create))
                            {
                                await response.Content.CopyToAsync(fileStream);
                            }

                            using (var response = await httpClient.GetAsync(highResUrl))
                            using (var fileStream = new FileStream(highResPath, FileMode.Create))
                            {
                                await response.Content.CopyToAsync(fileStream);
                            }

                            thumbnailUrl = $"/images/thumbnail_{imageId}.jpg";
                            mediumUrl = $"/images/medium_{imageId}.jpg";
                            highResUrl = $"/images/highres_{imageId}.jpg";

                            string description = $"{obj.title} - {obj.objectDate}\n\n{obj.medium}\n\n{obj.artistDisplayName} tarafından yapılan bu eser, {obj.period} dönemine aittir.";
                            if (string.IsNullOrWhiteSpace(description) || description == " -  -  - ")
                                description = "Bu eser hakkında detaylı bilgi bulunmamaktadır.";

                            decimal price = new Random().Next(1000, 10001);

                            // Eserin kategorisini belirle
                            Category category = defaultCategory;
                            if (obj.title.ToLower().Contains("portrait") || obj.title.ToLower().Contains("figure"))
                            {
                                category = categories["Portre ve Figür"];
                            }
                            else if (obj.title.ToLower().Contains("landscape") || obj.title.ToLower().Contains("nature"))
                            {
                                category = categories["Manzara ve Doğa"];
                            }
                            else if (obj.title.ToLower().Contains("still life") || obj.title.ToLower().Contains("nature morte"))
                            {
                                category = categories["Natürmort"];
                            }
                            else if (obj.title.ToLower().Contains("historical") || obj.title.ToLower().Contains("religious"))
                            {
                                category = categories["Tarihsel ve Dini"];
                            }

                            var artistEntity = context.Artists.First(a => a.Name == artist.Value.Name && a.Surname == artist.Value.Surname);
                            var artwork = new Artwork
                            {
                                Title = obj.title,
                                Description = description,
                                ImageUrl = thumbnailUrl,
                                MediumImageUrl = mediumUrl,
                                HighResImageUrl = highResUrl,
                                Price = price,
                                CategoryId = category.Id,
                                ArtistId = artistEntity.Id
                            };

                            context.Artworks.Add(artwork);
                            addedTitles.Add(obj.title);
                            added++;
                            Console.WriteLine($"Added artwork: {obj.title} by {artist.Key}");

                            // Her 5 eserde bir veritabanını kaydet
                            if (added % 5 == 0)
                            {
                                await context.SaveChangesAsync();
                                Console.WriteLine($"Saved {added} artworks so far");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing artwork {id}: {ex.Message}");
                            continue;
                        }
                    }
                }

                // Son değişiklikleri kaydet
                await context.SaveChangesAsync();
                Console.WriteLine($"Successfully added {added} artworks to the database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MetMuseumSeeder error: {ex}");
                throw;
            }
        }

        private class MetSearchResult
        {
            public int total { get; set; }
            public required List<int> objectIDs { get; set; }
        }

        private class MetObject
        {
            public string? title { get; set; }
            public string? primaryImage { get; set; }
            public string? objectDate { get; set; }
            public string? medium { get; set; }
            public string? period { get; set; }
            public string? artistDisplayName { get; set; }
            public string? classification { get; set; }
        }
    }
} 