using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Ödeme yöntemi seçilmesi zorunludur")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required(ErrorMessage = "Kargo adresi zorunludur")]
        [MinLength(10, ErrorMessage = "Kargo adresi en az 10 karakter olmalıdır")]
        public string? ShippingAddress { get; set; }

        // Kredi Kartı Bilgileri
        public string? CardNumber { get; set; }
        public string? Expiry { get; set; }
        public string? CVV { get; set; }
        public string? CardHolder { get; set; }
        public int InstallmentCount { get; set; } = 1;

        // Banka Transferi Bilgileri
        public string? BankAccountNumber { get; set; }

        // Kripto Para Bilgileri
        public string? CryptoWalletAddress { get; set; }

        // Hediye Kartı Bilgileri
        public string? GiftCardCode { get; set; }

        // İndirim Kodu
        public string? CouponCode { get; set; }
    }
} 