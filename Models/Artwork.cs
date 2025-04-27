using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class Artwork
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string HighResImageUrl { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
} 