using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı veya e-posta adresi gereklidir.")]
        [Display(Name = "Kullanıcı Adı veya E-posta")]
        public required string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public required string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
} 