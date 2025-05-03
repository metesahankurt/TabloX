using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TabloX2.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Telefon alanı zorunludur.")]
        [Display(Name = "Telefon")]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "Adres alanı zorunludur.")]
        [Display(Name = "Adres")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "Şehir alanı zorunludur.")]
        [Display(Name = "Şehir")]
        public required string City { get; set; }

        [Required(ErrorMessage = "İlçe alanı zorunludur.")]
        [Display(Name = "İlçe")]
        public required string District { get; set; }

        [Required(ErrorMessage = "Posta kodu alanı zorunludur.")]
        [Display(Name = "Posta Kodu")]
        public required string PostalCode { get; set; }

        [Required(ErrorMessage = "Ödeme yöntemi seçiniz.")]
        [Display(Name = "Ödeme Yöntemi")]
        public required string PaymentMethod { get; set; }

        // Kredi Kartı
        [Display(Name = "Kart Numarası")]
        public string? CardNumber { get; set; }
        [Display(Name = "Son Kullanma Tarihi (AA/YY)")]
        public string? CardExpiry { get; set; }
        [Display(Name = "CVV")]
        public string? CardCvv { get; set; }
        [Display(Name = "Kart Üzerindeki İsim")]
        public string? CardHolder { get; set; }

        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }
} 