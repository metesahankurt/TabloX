using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class Artwork
    {
        public int Id { get; set; }
        [Required]
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string ImageUrl { get; set; }
        public required string MediumImageUrl { get; set; }
        public required string HighResImageUrl { get; set; }
        public decimal Price { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }
        public int SalesCount { get; set; } = 0;
    }
} 