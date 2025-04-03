using System.ComponentModel.DataAnnotations;

namespace TabloX.Models
{
    public class ArtistProfile
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        [StringLength(500)]
        public required string Bio { get; set; }

        [Url]
        public string? Website { get; set; }

        [StringLength(50)]
        public string? Instagram { get; set; }

        [StringLength(50)]
        public string? Facebook { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
} 