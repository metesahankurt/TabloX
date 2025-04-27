using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ArtworkId { get; set; }
        public Artwork Artwork { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
} 