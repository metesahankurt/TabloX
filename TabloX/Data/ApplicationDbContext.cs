using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TabloX.Models;
using System;

namespace TabloX.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ArtistProfile> ArtistProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.Artist)
                .WithMany()
                .HasForeignKey(p => p.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<ArtistProfile>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<ArtistProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Yağlı Boya" },
                new Category { Id = 2, Name = "Empresyonist" },
                new Category { Id = 3, Name = "Modern" },
                new Category { Id = 4, Name = "Klasik" },
                new Category { Id = 5, Name = "Portre" },
                new Category { Id = 6, Name = "Soyut" },
                new Category { Id = 7, Name = "Manzara" },
                new Category { Id = 8, Name = "Sürrealist" }
            );

            // Seed Products
            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Wheat Field with Cypresses",
                    Description = "Vincent van Gogh'un ünlü eseri, buğday tarlası ve servi ağaçlarının muhteşem uyumu",
                    Price = 2500.00M,
                    ImageUrl = "/images/artworks/wheat_field_with_cypresses.jpg",
                    CategoryId = 1,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 2,
                    Title = "Irises",
                    Description = "Van Gogh'un süsenleri konu alan muhteşem eseri",
                    Price = 2200.00M,
                    ImageUrl = "/images/artworks/irises.jpg",
                    CategoryId = 1,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 3,
                    Title = "Still Life with Apples",
                    Description = "Paul Cézanne'in klasik natürmort çalışması",
                    Price = 1800.00M,
                    ImageUrl = "/images/artworks/still_life_with_apples.jpg",
                    CategoryId = 4,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 4,
                    Title = "Water Lilies",
                    Description = "Claude Monet'nin ünlü nilüfer serilerinden etkileyici bir eser",
                    Price = 2300.00M,
                    ImageUrl = "/images/artworks/water_lilies.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 5,
                    Title = "Starry Night",
                    Description = "Van Gogh'un en ünlü eserlerinden biri, yıldızlı bir gece manzarası",
                    Price = 3000.00M,
                    ImageUrl = "/images/artworks/starry_night.jpg",
                    CategoryId = 1,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 6,
                    Title = "Girl with a Pearl Earring",
                    Description = "Vermeer'in en tanınmış portre çalışması",
                    Price = 2100.00M,
                    ImageUrl = "/images/artworks/girl_with_pearl_earring.jpg",
                    CategoryId = 5,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 7,
                    Title = "The Birth of Venus",
                    Description = "Botticelli'nin mitolojik konulu başyapıtı",
                    Price = 2800.00M,
                    ImageUrl = "/images/artworks/birth_of_venus.jpg",
                    CategoryId = 4,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 8,
                    Title = "The Night Watch",
                    Description = "Rembrandt'ın ışık ve gölge ustası olduğunu gösteren eseri",
                    Price = 2600.00M,
                    ImageUrl = "/images/artworks/night_watch.jpg",
                    CategoryId = 4,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 9,
                    Title = "The Scream",
                    Description = "Edvard Munch'un modern sanatın sembolü haline gelen eseri",
                    Price = 2400.00M,
                    ImageUrl = "/images/artworks/the_scream.jpg",
                    CategoryId = 3,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 10,
                    Title = "The Last Supper",
                    Description = "Leonardo da Vinci'nin dini konulu ünlü freskosu",
                    Price = 3500.00M,
                    ImageUrl = "/images/artworks/last_supper.jpg",
                    CategoryId = 4,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 11,
                    Title = "The Garden of Earthly Delights",
                    Description = "Hieronymus Bosch'un fantastik ve alegorik başyapıtı",
                    Price = 2900.00M,
                    ImageUrl = "/images/artworks/garden_of_earthly_delights.jpg",
                    CategoryId = 8,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 12,
                    Title = "The Kiss",
                    Description = "Gustav Klimt'in altın yapraklı romantik eseri",
                    Price = 2400.00M,
                    ImageUrl = "/images/artworks/the_kiss.jpg",
                    CategoryId = 4,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 13,
                    Title = "Liberty Leading the People",
                    Description = "Eugène Delacroix'nın devrim temalı tablosu",
                    Price = 2700.00M,
                    ImageUrl = "/images/artworks/liberty_leading_the_people.jpg",
                    CategoryId = 4,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 14,
                    Title = "The Great Wave off Kanagawa",
                    Description = "Katsushika Hokusai'nin ünlü Japon baskı sanatı örneği",
                    Price = 1600.00M,
                    ImageUrl = "/images/artworks/great_wave.jpg",
                    CategoryId = 7,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 15,
                    Title = "The Sleeping Gypsy",
                    Description = "Henri Rousseau'nun primitif stil örneği",
                    Price = 1900.00M,
                    ImageUrl = "/images/artworks/sleeping_gypsy.jpg",
                    CategoryId = 3,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 16,
                    Title = "The Arnolfini Portrait",
                    Description = "Jan van Eyck'in detaylı evlilik portresi",
                    Price = 2200.00M,
                    ImageUrl = "/images/artworks/arnolfini_portrait.jpg",
                    CategoryId = 5,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 17,
                    Title = "Sunflowers",
                    Description = "Van Gogh'un ünlü ayçiçeği serisi",
                    Price = 2300.00M,
                    ImageUrl = "/images/artworks/sunflowers.jpg",
                    CategoryId = 1,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 18,
                    Title = "Café Terrace at Night",
                    Description = "Van Gogh'un gece kafesi manzarası",
                    Price = 2100.00M,
                    ImageUrl = "/images/artworks/cafe_terrace.jpg",
                    CategoryId = 7,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 19,
                    Title = "Impression, Sunrise",
                    Description = "Claude Monet'nin empresyonizme adını veren eseri",
                    Price = 2500.00M,
                    ImageUrl = "/images/artworks/impression_sunrise.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 20,
                    Title = "Haystacks",
                    Description = "Monet'nin farklı ışık koşullarında saman balyaları",
                    Price = 2000.00M,
                    ImageUrl = "/images/artworks/haystacks.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 21,
                    Title = "Poppy Field",
                    Description = "Monet'nin renkli gelincik tarlası",
                    Price = 1900.00M,
                    ImageUrl = "/images/artworks/poppy_field.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 22,
                    Title = "The Dance Class",
                    Description = "Edgar Degas'nın bale sınıfı çalışması",
                    Price = 2200.00M,
                    ImageUrl = "/images/artworks/dance_class.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 23,
                    Title = "The Card Players",
                    Description = "Paul Cézanne'in kağıt oynayanlar serisi",
                    Price = 2400.00M,
                    ImageUrl = "/images/artworks/card_players.jpg",
                    CategoryId = 4,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 24,
                    Title = "Mont Sainte-Victoire",
                    Description = "Cézanne'in sevdiği dağın portresi",
                    Price = 2100.00M,
                    ImageUrl = "/images/artworks/mont_sainte_victoire.jpg",
                    CategoryId = 7,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 25,
                    Title = "Blue Dancers",
                    Description = "Degas'nın mavi tonlardaki balerinleri",
                    Price = 2300.00M,
                    ImageUrl = "/images/artworks/blue_dancers.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 26,
                    Title = "L'Absinthe",
                    Description = "Degas'nın Paris cafe yaşamını yansıtan eseri",
                    Price = 2000.00M,
                    ImageUrl = "/images/artworks/absinthe_drinker.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 27,
                    Title = "Self Portrait",
                    Description = "Van Gogh'un kendini resmettiği otoportre",
                    Price = 2600.00M,
                    ImageUrl = "/images/artworks/self_portrait.jpg",
                    CategoryId = 5,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 28,
                    Title = "The Potato Eaters",
                    Description = "Van Gogh'un erken dönem başyapıtı",
                    Price = 2200.00M,
                    ImageUrl = "/images/artworks/potato_eaters.jpg",
                    CategoryId = 4,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 29,
                    Title = "Bedroom in Arles",
                    Description = "Van Gogh'un yatak odası serisi",
                    Price = 2100.00M,
                    ImageUrl = "/images/artworks/bedroom_arles.jpg",
                    CategoryId = 3,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 30,
                    Title = "The Japanese Bridge",
                    Description = "Monet'nin bahçesindeki Japon köprüsü",
                    Price = 2400.00M,
                    ImageUrl = "/images/artworks/water_lilies.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 31,
                    Title = "Rouen Cathedral Series",
                    Description = "Monet'nin katedral serisi",
                    Price = 2500.00M,
                    ImageUrl = "/images/artworks/rouen_cathedral.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 32,
                    Title = "A Bar at the Folies-Bergère",
                    Description = "Manet'nin modern yaşam portresi",
                    Price = 2300.00M,
                    ImageUrl = "/images/artworks/bar_folies_bergere.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 33,
                    Title = "Olympia",
                    Description = "Manet'nin tartışmalı başyapıtı",
                    Price = 2700.00M,
                    ImageUrl = "/images/artworks/olympia.jpg",
                    CategoryId = 4,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 34,
                    Title = "Le Déjeuner sur l'herbe",
                    Description = "Manet'nin modern piknik sahnesi",
                    Price = 2600.00M,
                    ImageUrl = "/images/artworks/luncheon_grass.jpg",
                    CategoryId = 4,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 35,
                    Title = "The Dancing Class",
                    Description = "Degas'nın dans sınıfı serisi",
                    Price = 2200.00M,
                    ImageUrl = "/images/artworks/dancing_class.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 36,
                    Title = "Ballet Rehearsal",
                    Description = "Degas'nın bale provası sahnesi",
                    Price = 2100.00M,
                    ImageUrl = "/images/artworks/ballet_rehearsal.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 37,
                    Title = "Circus Sideshow",
                    Description = "Seurat'nın sirk gösterisi",
                    Price = 2000.00M,
                    ImageUrl = "/images/artworks/circus_sideshow.jpg",
                    CategoryId = 3,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 38,
                    Title = "A Sunday Afternoon",
                    Description = "Seurat'nın puantilist başyapıtı",
                    Price = 2800.00M,
                    ImageUrl = "/images/artworks/sunday_afternoon.jpg",
                    CategoryId = 3,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 39,
                    Title = "Bal du moulin de la Galette",
                    Description = "Renoir'ın Paris yaşamını yansıtan eseri",
                    Price = 2500.00M,
                    ImageUrl = "/images/artworks/moulin_galette.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 40,
                    Title = "Luncheon of the Boating Party",
                    Description = "Renoir'ın Seine nehri kıyısındaki öğle yemeği sahnesi",
                    Price = 2600.00M,
                    ImageUrl = "/images/artworks/dance_bougival.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 41,
                    Title = "Dance at Bougival",
                    Description = "Renoir'ın dans eden çift portresi",
                    Price = 2200.00M,
                    ImageUrl = "/images/artworks/dance_bougival.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 42,
                    Title = "Madame Charpentier",
                    Description = "Renoir'ın aristokrat portresi",
                    Price = 2300.00M,
                    ImageUrl = "/images/artworks/adele.jpg",
                    CategoryId = 5,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 43,
                    Title = "Ballet Stage",
                    Description = "Degas'nın sahne arkası çalışması",
                    Price = 2100.00M,
                    ImageUrl = "/images/artworks/dance_class.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 44,
                    Title = "Race Horses",
                    Description = "Degas'nın at yarışı sahnesi",
                    Price = 2000.00M,
                    ImageUrl = "/images/artworks/mont_sainte_victoire.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 45,
                    Title = "The Umbrella",
                    Description = "Renoir'ın şemsiyeli kadın portresi",
                    Price = 2200.00M,
                    ImageUrl = "/images/artworks/self_portrait.jpg",
                    CategoryId = 5,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 46,
                    Title = "Theater Box",
                    Description = "Renoir'ın tiyatro locası sahnesi",
                    Price = 2300.00M,
                    ImageUrl = "/images/artworks/bar_folies_bergere.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 47,
                    Title = "Bridge at Argenteuil",
                    Description = "Monet'nin Seine nehri üzerindeki köprü manzarası",
                    Price = 2200.00M,
                    ImageUrl = "/images/artworks/impression_sunrise.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 48,
                    Title = "Gare Saint-Lazare",
                    Description = "Monet'nin tren istasyonu serisi",
                    Price = 2200.00M,
                    ImageUrl = "/images/artworks/gare_saint_lazare.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 49,
                    Title = "Spring Flowers",
                    Description = "Renoir'ın çiçek natürmortu",
                    Price = 1900.00M,
                    ImageUrl = "/images/artworks/cicek_bahcesi.jpg",
                    CategoryId = 1,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 50,
                    Title = "Young Girls at the Piano",
                    Description = "Renoir'ın müzisyen portresi",
                    Price = 2100.00M,
                    ImageUrl = "/images/artworks/adele.jpg",
                    CategoryId = 5,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 51,
                    Title = "Autumn Effect at Argenteuil",
                    Description = "Claude Monet'nin sonbahar manzarası, Argenteuil'deki ağaçların ve gökyüzünün muhteşem renkleri",
                    Price = 2400.00M,
                    ImageUrl = "/images/artworks/autumn_effect.jpg",
                    CategoryId = 2,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
} 