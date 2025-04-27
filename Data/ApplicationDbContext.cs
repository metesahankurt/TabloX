using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TabloX2.Models;

namespace TabloX2.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TabloX2.Models.Category> Categories { get; set; }
    public DbSet<TabloX2.Models.Artist> Artists { get; set; }
    public DbSet<TabloX2.Models.Artwork> Artworks { get; set; }
    public DbSet<TabloX2.Models.Order> Orders { get; set; }
    public DbSet<TabloX2.Models.OrderItem> OrderItems { get; set; }
    public DbSet<TabloX2.Models.CartItem> CartItems { get; set; }
}
