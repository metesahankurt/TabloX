using System.ComponentModel.DataAnnotations;

namespace TabloX2.Models.ViewModels
{
    public class TwoFactorAuthenticationViewModel
    {
        public string? SharedKey { get; set; }
        public string? AuthenticatorUri { get; set; }
        public string[]? RecoveryCodes { get; set; }

        [Required(ErrorMessage = "Doğrulama kodu gereklidir")]
        [StringLength(7, MinimumLength = 6, ErrorMessage = "Doğrulama kodu 6 haneli olmalıdır")]
        [Display(Name = "Doğrulama Kodu")]
        public string? Code { get; set; }

        public bool Is2faEnabled { get; set; }
        public bool IsMachineRemembered { get; set; }
    }

    public class VerifyTwoFactorCodeViewModel
    {
        [Required(ErrorMessage = "Doğrulama kodu gereklidir")]
        [StringLength(7, MinimumLength = 6, ErrorMessage = "Doğrulama kodu 6 haneli olmalıdır")]
        [Display(Name = "Doğrulama Kodu")]
        public string? Code { get; set; }
        public bool RememberMachine { get; set; }
    }
} 