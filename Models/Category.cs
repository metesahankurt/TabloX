using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Artwork> Artworks { get; set; }
    }
} 