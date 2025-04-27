using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TabloX2.Models;
using System.Text.Json.Serialization;

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

                // Anahtar kelimelerle arama
                var keywords = new List<string> {
                    "Van Gogh", "Monet", "Rembrandt", "Picasso", "Da Vinci", "Klimt", "Cézanne", "Matisse", "Goya", "Vermeer", "Dali", "Munch", "Renoir", "Degas", "Raphael", "Botticelli", "Michelangelo", "Turner", "Frida Kahlo", "Painting"
                };
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                using var httpClient = new HttpClient(handler);

                var artworks = new List<Artwork>();
                var addedTitles = new HashSet<string>();
                int added = 0;
                var allowedArtists = new HashSet<string> {
                    "Vincent van Gogh",
                    "Auguste Renoir",
                    "Adolphe Monticelli",
                    "Rembrandt (Rembrandt van Rijn)",
                    "Philippe Rousseau",
                    "Eugène Delacroix",
                    "Camille Pissarro",
                    "Jean Michelin",
                    "Edouard Manet",
                    "Paul Cézanne",
                    "Domenico Fetti",
                    "Anton Mauve",
                    "Jean-François Millet",
                    "Charles Bargue",
                    "Charles-François Daubigny",
                    "Nicolaes Maes",
                    "Camille Corot",
                    "Théodore Rousseau",
                    "Théodule-Augustin Ribot",
                    "Edgar Degas",
                    "Henri de Toulouse-Lautrec",
                    "Honoré Daumier",
                    "Odilon Redon",
                    "Alfred Stevens"
                };

                // Sanatçı biyografileri ve kullanıcı adları
                var artistBios = new Dictionary<string, string> {
                    { "Vincent van Gogh", "Hollandalı post-empresyonist ressam. 1853-1890 yılları arasında yaşamış, kısa ömründe sanat tarihinin en etkili isimlerinden biri olmuştur. Eserlerinde duygusal yoğunluk ve canlı renkler ön plandadır." },
                    { "Auguste Renoir", "Fransız empresyonist ressam. Renk ve ışık kullanımıyla tanınır, portre ve günlük yaşam sahneleriyle ünlüdür. 1841-1919 yılları arasında yaşamıştır." },
                    { "Adolphe Monticelli", "Fransız ressam. Marseilles 1824–1886. Renkli ve dokulu fırça darbeleriyle tanınır." },
                    { "Rembrandt (Rembrandt van Rijn)", "Hollandalı barok ressam ve gravür sanatçısı. Işık-gölge kullanımı ve portreleriyle ünlüdür. 1606-1669." },
                    { "Philippe Rousseau", "Fransız ressam. Paris 1816–1887 Acquigny. Özellikle natürmortlarıyla tanınır." },
                    { "Eugène Delacroix", "Fransız romantik ressam. Renk ve hareketin ustası olarak kabul edilir. 1798-1863." },
                    { "Camille Pissarro", "Danimarka asıllı Fransız empresyonist ressam. Doğa ve köy yaşamı temalı eserleriyle bilinir. 1830-1903." },
                    { "Jean Michelin", "Fransız ressam. 1616–1670. Barok dönemi eserleriyle tanınır." },
                    { "Edouard Manet", "Fransız ressam. Empresyonizmin öncülerindendir, modern yaşamı konu alan eserleriyle tanınır. 1832-1883." },
                    { "Paul Cézanne", "Fransız post-empresyonist ressam. Modern sanatın öncülerindendir, doğa ve nesne analizleriyle bilinir. 1839-1906." },
                    { "Domenico Fetti", "İtalyan ressam. Roma 1591/92–1623 Venedik. Barok dönemi eserleriyle tanınır." },
                    { "Anton Mauve", "Hollandalı ressam. 1838–1888. Manzara ve köy yaşamı temalı eserleriyle bilinir." },
                    { "Jean-François Millet", "Fransız ressam. 1814–1875. Köylü yaşamını ve doğayı konu alan eserleriyle tanınır." },
                    { "Charles Bargue", "Fransız ressam ve gravür sanatçısı. 1825/26–1883. Akademik sanat eğitimiyle bilinir." },
                    { "Charles-François Daubigny", "Fransız ressam. 1817–1878. Barbizon ekolünün önemli temsilcilerindendir." },
                    { "Nicolaes Maes", "Hollandalı ressam. 1634–1693. Portre ve günlük yaşam sahneleriyle tanınır." },
                    { "Camille Corot", "Fransız ressam. 1796–1875. Manzara resminin öncülerindendir." },
                    { "Théodore Rousseau", "Fransız ressam. 1812–1867. Barbizon ekolünün kurucularındandır." },
                    { "Théodule-Augustin Ribot", "Fransız ressam. 1823–1891. Natürmort ve portreleriyle tanınır." },
                    { "Edgar Degas", "Fransız empresyonist ressam ve heykeltıraş. Özellikle balerinleri ve hareketi betimleyen eserleriyle tanınır. 1834-1917." },
                    { "Henri de Toulouse-Lautrec", "Fransız ressam, illüstratör ve litograf. Paris gece hayatını ve kabare sahnelerini ölümsüzleştirmiştir. 1864-1901." },
                    { "Honoré Daumier", "Fransız ressam, karikatürist ve heykeltıraş. Toplumsal eleştirileriyle tanınır. 1808-1879." },
                    { "Odilon Redon", "Fransız sembolist ressam. 1840–1916. Hayal gücüne dayalı eserleriyle bilinir." },
                    { "Alfred Stevens", "Belçikalı ressam. Özellikle kadın portreleriyle tanınır. 1823–1906." }
                };

                // Sanatçı ülkeleri sözlüğü
                var artistCountries = new Dictionary<string, string> {
                    { "Vincent van Gogh", "Hollanda" },
                    { "Auguste Renoir", "Fransa" },
                    { "Adolphe Monticelli", "Fransa" },
                    { "Rembrandt (Rembrandt van Rijn)", "Hollanda" },
                    { "Philippe Rousseau", "Fransa" },
                    { "Eugène Delacroix", "Fransa" },
                    { "Camille Pissarro", "Fransa" },
                    { "Jean Michelin", "Fransa" },
                    { "Edouard Manet", "Fransa" },
                    { "Paul Cézanne", "Fransa" },
                    { "Domenico Fetti", "İtalya" },
                    { "Anton Mauve", "Hollanda" },
                    { "Jean-François Millet", "Fransa" },
                    { "Charles Bargue", "Fransa" },
                    { "Charles-François Daubigny", "Fransa" },
                    { "Nicolaes Maes", "Hollanda" },
                    { "Camille Corot", "Fransa" },
                    { "Théodore Rousseau", "Fransa" },
                    { "Théodule-Augustin Ribot", "Fransa" },
                    { "Edgar Degas", "Fransa" },
                    { "Henri de Toulouse-Lautrec", "Fransa" },
                    { "Honoré Daumier", "Fransa" },
                    { "Odilon Redon", "Fransa" },
                    { "Alfred Stevens", "Belçika" }
                };

                // Önce sanatçıları ekle
                foreach (var artistName in allowedArtists)
                {
                    if (context.Artists.Any(a => a.Name == artistName)) continue;
                    string profileImg = $"https://ui-avatars.com/api/?name={System.Web.HttpUtility.UrlEncode(artistName)}&background=0D8ABC&color=fff";
                    string artistBio = artistBios.ContainsKey(artistName) ? artistBios[artistName] : "";
                    string country = artistCountries.ContainsKey(artistName) ? artistCountries[artistName] : "Bilinmiyor";
                    var artistEntity = new Artist { Name = artistName, Bio = artistBio, ProfileImageUrl = profileImg, Country = country };
                    context.Artists.Add(artistEntity);
                }
                context.SaveChanges();

                var rand = new Random();
                foreach (var keyword in keywords)
                {
                    if (added >= 48) break;
                    var searchUrl = $"https://collectionapi.metmuseum.org/public/collection/v1/search?hasImages=true&q={System.Web.HttpUtility.UrlEncode(keyword)}";
                    var searchResponse = await httpClient.GetStringAsync(searchUrl);
                    var searchObj = JsonSerializer.Deserialize<MetSearchResult>(searchResponse);
                    var objectIds = searchObj?.objectIDs ?? new List<int>();
                    foreach (var id in objectIds)
                    {
                        if (added >= 48) break;
                        string objResponse = null;
                        try
                        {
                            objResponse = await httpClient.GetStringAsync($"https://collectionapi.metmuseum.org/public/collection/v1/objects/{id}");
                        }
                        catch
                        {
                            continue; // 404 veya başka hata olursa atla
                        }
                        var obj = JsonSerializer.Deserialize<MetObject>(objResponse);
                        if (obj == null || string.IsNullOrEmpty(obj.primaryImage) || string.IsNullOrEmpty(obj.title)) continue;
                        if (addedTitles.Contains(obj.title)) continue; // Aynı başlık eklenmesin
                        if (!string.IsNullOrEmpty(obj.classification) && !obj.classification.ToLower().Contains("painting")) continue;
                        string description = $"{obj.objectName} - {obj.culture} - {obj.period} - {obj.medium}";
                        if (string.IsNullOrWhiteSpace(description) || description == " -  -  - ")
                            description = "Açıklama bulunamadı.";
                        string artist = string.IsNullOrEmpty(obj.artistDisplayName) ? "Bilinmeyen" : obj.artistDisplayName;
                        if (!allowedArtists.Contains(artist)) continue; // Sadece izin verilen sanatçılar eklensin
                        string artistBio = string.IsNullOrEmpty(obj.artistDisplayBio) ? "" : obj.artistDisplayBio;
                        decimal price = rand.Next(1000, 10001); // 1000-10000 TL arası rastgele fiyat
                        string yearInfo = string.IsNullOrWhiteSpace(obj.objectEndDate) ? "yıl bilgisi yok" : obj.objectEndDate;
                        string promo = $"Bu eser {artist} tarafından {yearInfo} yılında yapılmıştır. {description}";

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
                        else if (obj.title.ToLower().Contains("modern") || obj.title.ToLower().Contains("contemporary"))
                        {
                            category = categories["Modern Yaşam"];
                        }
                        var artistEntity = context.Artists.FirstOrDefault(a => a.Name == artist);
                        var artwork = new Artwork
                        {
                            Title = obj.title,
                            Description = promo,
                            ImageUrl = obj.primaryImage,
                            HighResImageUrl = obj.primaryImage,
                            Price = price,
                            Category = category,
                            Artist = artistEntity
                        };
                        context.Artworks.Add(artwork);
                        addedTitles.Add(obj.title);
                        added++;
                    }
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MetMuseumSeeder hata: {ex}");
            }
        }

        private class MetSearchResult
        {
            public int total { get; set; }
            public List<int> objectIDs { get; set; }
        }
        private class MetObject
        {
            public string? title { get; set; }
            public string? primaryImage { get; set; }
            public string? primaryImageSmall { get; set; }
            public string? objectName { get; set; }
            public string? culture { get; set; }
            public string? period { get; set; }
            public string? medium { get; set; }
            public string? artistDisplayName { get; set; }
            public string? artistDisplayBio { get; set; }
            public string? classification { get; set; }
            [JsonConverter(typeof(FlexibleStringConverter))]
            public string? objectEndDate { get; set; }
        }

        // FlexibleStringConverter: int veya string gelen JSON'u string'e çevirir
        public class FlexibleStringConverter : JsonConverter<string>
        {
            public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                    return reader.GetString();
                if (reader.TokenType == JsonTokenType.Number)
                    return reader.GetInt32().ToString();
                return null;
            }
            public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value);
            }
        }

        // Sanatçı profil görseli ve biyografi sözlüğü
        private static readonly Dictionary<string, (string img, string bio)> artistProfiles = new Dictionary<string, (string img, string bio)>
        {
            { "Vincent van Gogh", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Hollandalı post-empresyonist ressam. 1853-1890 yılları arasında yaşamış, kısa ömründe sanat tarihinin en etkili isimlerinden biri olmuştur. Eserlerinde duygusal yoğunluk ve canlı renkler ön plandadır.") },
            { "Auguste Renoir", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız empresyonist ressam. Renk ve ışık kullanımıyla tanınır, portre ve günlük yaşam sahneleriyle ünlüdür. 1841-1919 yılları arasında yaşamıştır.") },
            { "Adolphe Monticelli", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam. Marseilles 1824–1886. Renkli ve dokulu fırça darbeleriyle tanınır.") },
            { "Rembrandt (Rembrandt van Rijn)", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Hollandalı barok ressam ve gravür sanatçısı. Işık-gölge kullanımı ve portreleriyle ünlüdür. 1606-1669.") },
            { "Philippe Rousseau", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam. Paris 1816–1887 Acquigny. Özellikle natürmortlarıyla tanınır.") },
            { "Eugène Delacroix", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız romantik ressam. Renk ve hareketin ustası olarak kabul edilir. 1798-1863.") },
            { "Camille Pissarro", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Danimarka asıllı Fransız empresyonist ressam. Doğa ve köy yaşamı temalı eserleriyle bilinir. 1830-1903.") },
            { "Jean Michelin", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam. 1616–1670. Barok dönemi eserleriyle tanınır.") },
            { "Edouard Manet", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam. Empresyonizmin öncülerindendir, modern yaşamı konu alan eserleriyle tanınır. 1832-1883.") },
            { "Paul Cézanne", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız post-empresyonist ressam. Modern sanatın öncülerindendir, doğa ve nesne analizleriyle bilinir. 1839-1906.") },
            { "Domenico Fetti", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "İtalyan ressam. Roma 1591/92–1623 Venedik. Barok dönemi eserleriyle tanınır.") },
            { "Anton Mauve", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Hollandalı ressam. 1838–1888. Manzara ve köy yaşamı temalı eserleriyle bilinir.") },
            { "Jean-François Millet", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam. 1814–1875. Köylü yaşamını ve doğayı konu alan eserleriyle tanınır.") },
            { "Charles Bargue", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam ve gravür sanatçısı. 1825/26–1883. Akademik sanat eğitimiyle bilinir.") },
            { "Charles-François Daubigny", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam. 1817–1878. Barbizon ekolünün önemli temsilcilerindendir.") },
            { "Nicolaes Maes", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Hollandalı ressam. 1634–1693. Portre ve günlük yaşam sahneleriyle tanınır.") },
            { "Camille Corot", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam. 1796–1875. Manzara resminin öncülerindendir.") },
            { "Théodore Rousseau", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam. 1812–1867. Barbizon ekolünün kurucularındandır.") },
            { "Théodule-Augustin Ribot", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam. 1823–1891. Natürmort ve portreleriyle tanınır.") },
            { "Edgar Degas", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız empresyonist ressam ve heykeltıraş. Özellikle balerinleri ve hareketi betimleyen eserleriyle tanınır. 1834-1917.") },
            { "Henri de Toulouse-Lautrec", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam, illüstratör ve litograf. Paris gece hayatını ve kabare sahnelerini ölümsüzleştirmiştir. 1864-1901.") },
            { "Honoré Daumier", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız ressam, karikatürist ve heykeltıraş. Toplumsal eleştirileriyle tanınır. 1808-1879.") },
            { "Odilon Redon", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Fransız sembolist ressam. 1840–1916. Hayal gücüne dayalı eserleriyle bilinir.") },
            { "Alfred Stevens", ("https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png", "Belçikalı ressam. Özellikle kadın portreleriyle tanınır. 1823–1906.") },
        };
    }
} 