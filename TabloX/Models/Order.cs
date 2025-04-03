using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TabloX.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        [Required]
        public string Status { get; set; } = null!;
        [Required]
        public string OrderCode { get; set; } = null!;

        [Required(ErrorMessage = "Kargo adresi gereklidir.")]
        public string ShippingAddress { get; set; } = null!;

        [Required(ErrorMessage = "Fatura adresi gereklidir.")]
        public string BillingAddress { get; set; } = null!;

        [Required(ErrorMessage = "Kart sahibinin adı gereklidir.")]
        public string CardHolderName { get; set; } = null!;

        [Required(ErrorMessage = "Kart numarası gereklidir.")]
        [RegularExpression(@"^[\d\s]+$", ErrorMessage = "Geçerli bir kart numarası giriniz.")]
        public string CardNumber { get; set; } = null!;

        [Required(ErrorMessage = "Son kullanma tarihi gereklidir.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Geçerli bir son kullanma tarihi giriniz (AA/YY).")]
        public string ExpiryDate { get; set; } = null!;

        [Required(ErrorMessage = "CVV gereklidir.")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Geçerli bir CVV giriniz.")]
        public string CVV { get; set; } = null!;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
} 