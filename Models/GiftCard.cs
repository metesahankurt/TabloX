using System;
using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class GiftCard
    {
        public int Id { get; set; }

        [Required]
        [StringLength(16)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UsedDate { get; set; }

        public string? CreatedBy { get; set; }

        public string? UsedBy { get; set; }

        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddYears(1);
    }
} 