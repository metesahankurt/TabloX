using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ArtworkId { get; set; }
        public Artwork Artwork { get; set; }
        public int Quantity { get; set; }
    }
} 