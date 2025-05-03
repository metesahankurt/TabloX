using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TabloX2.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public required string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        public required string ShippingAddress { get; set; }

        [Required]
        public required string PhoneNumber { get; set; }

        [Required]
        public required string ShippingStatus { get; set; }

        public OrderStatus Status { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        [Display(Name = "Sipariş Alındı")]
        Alindi,

        [Display(Name = "Sipariş Hazırlanıyor")]
        Hazirlaniyor,

        [Display(Name = "Kargoda")]
        Kargoda,

        [Display(Name = "Kargo Yola Çıktı")]
        Yolda,

        [Display(Name = "Teslim Edildi")]
        TeslimEdildi,

        [Display(Name = "İptal Edildi")]
        IptalEdildi
    }

    public enum PaymentMethod
    {
        [Display(Name = "Kredi Kartı")]
        CreditCard,

        [Display(Name = "Banka Transferi")]
        BankTransfer,

        [Display(Name = "Kripto Para")]
        CryptoCurrency,

        [Display(Name = "Hediye Kartı")]
        GiftCard
    }

    public static class OrderStatusExtensions
    {
        public static string GetDisplayName(this OrderStatus status)
        {
            var displayAttribute = status.GetType()
                .GetField(status.ToString())
                .GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? status.ToString();
        }
    }
} 