using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class Artist
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Bio { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Country { get; set; }
        public ICollection<Artwork> Artworks { get; set; }
    }
} 