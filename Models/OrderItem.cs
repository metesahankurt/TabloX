using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ArtworkId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public required virtual Order Order { get; set; }
        public required virtual Artwork Artwork { get; set; }
    }
} 