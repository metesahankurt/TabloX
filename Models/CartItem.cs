using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int ArtworkId { get; set; }
        public required Artwork Artwork { get; set; }
        public int Quantity { get; set; }
    }
} 