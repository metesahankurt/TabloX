using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class Artist
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public required string Surname { get; set; }

        [Required(ErrorMessage = "Biyografi alanı zorunludur.")]
        [Display(Name = "Biyografi")]
        public required string Bio { get; set; }

        [Display(Name = "Açıklama")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Ülke alanı zorunludur.")]
        [Display(Name = "Ülke")]
        public required string Country { get; set; }

        public virtual ICollection<Artwork>? Artworks { get; set; }
    }
} 