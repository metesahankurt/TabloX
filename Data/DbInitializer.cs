using Microsoft.AspNetCore.Identity;
using TabloX2.Models;
using System.Threading.Tasks;

namespace TabloX2.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Admin rolünü kontrol et ve oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Admin kullanıcısını kontrol et ve oluştur
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@tablox.com",
                    FirstName = "Admin",
                    LastName = "User",
                    FullName = "Admin User",
                    DisplayUserName = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
} 