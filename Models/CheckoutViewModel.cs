using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models
{
    public class CheckoutViewModel
    {
        [Required]
        [Display(Name = "Kart Numarası")]
        [CreditCard(ErrorMessage = "Geçerli bir kart numarası giriniz.")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Son Kullanma Tarihi (AA/YY)")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Geçerli bir tarih giriniz (AA/YY).")]
        public string Expiry { get; set; }

        [Required]
        [Display(Name = "CVV")]
        [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "Geçerli bir CVV giriniz.")]
        public string CVV { get; set; }

        [Required]
        [Display(Name = "Kart Üzerindeki İsim")]
        public string CardHolder { get; set; }

        [Required]
        [Display(Name = "Kargo Adresi")]
        public string ShippingAddress { get; set; }
    }
} 