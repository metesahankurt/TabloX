using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class Artist
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public required string Bio { get; set; }
        public required string ProfileImageUrl { get; set; }
        public required string Country { get; set; }
        public ICollection<Artwork> Artworks { get; set; } = new List<Artwork>();
    }
} 