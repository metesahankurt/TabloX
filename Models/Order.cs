using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        [Required]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        public string ShippingStatus { get; set; } = string.Empty;

        public PaymentMethod PaymentMethod { get; set; }

        public int InstallmentCount { get; set; }

        public string? MaskedCardNumber { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? CryptoWalletAddress { get; set; }

        public string? GiftCardCode { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FinalAmount { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ArtworkId { get; set; }
        public required Artwork Artwork { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public required Order Order { get; set; }
    }
} 