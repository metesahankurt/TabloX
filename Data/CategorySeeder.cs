using TabloX2.Models;

namespace TabloX2.Data
{
    public class CategorySeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Categories.Any())
            {
                return; // Veritabanında zaten kategoriler var
            }

            var categories = new List<Category>
            {
                new Category { Name = "Portre ve Figür" },
                new Category { Name = "Manzara ve Doğa" },
                new Category { Name = "Natürmort" },
                new Category { Name = "Tarihsel ve Dini" },
                new Category { Name = "Modern Yaşam" },
                new Category { Name = "Empresyonist" },
                new Category { Name = "Post-Empresyonist" },
                new Category { Name = "Barok ve Klasik" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
} 