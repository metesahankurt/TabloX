using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class Cart
    {
        public Cart()
        {
            Items = new List<CartItem>();
        }

        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        public decimal Total => Items.Sum(i => i.Artwork?.Price ?? 0);

        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<CartItem> Items { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public int ArtworkId { get; set; }

        [Required]
        public required string UserId { get; set; }

        public int Quantity { get; set; }

        public virtual Cart? Cart { get; set; }
        public virtual Artwork? Artwork { get; set; }
    }
} 