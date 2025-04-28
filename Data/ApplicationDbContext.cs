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
    public DbSet<TabloX2.Models.GiftCard> GiftCards { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Artwork)
            .WithMany()
            .HasForeignKey(oi => oi.ArtworkId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
