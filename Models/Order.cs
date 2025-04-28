using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        [Required]
        public string OrderNumber { get; set; }
        [Required]
        public string ShippingAddress { get; set; }
        [Required]
        public string MaskedCardNumber { get; set; }
        [Required]
        public string ShippingStatus { get; set; }
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