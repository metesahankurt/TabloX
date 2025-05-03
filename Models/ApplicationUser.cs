using Microsoft.AspNetCore.Identity;

namespace TabloX2.Models
{
    public class ApplicationUser : IdentityUser
    {
        // @kullaniciadi formatında kullanıcı adı
        public required string DisplayUserName { get; set; } // ör: @birseybirsey
        public required string FullName { get; set; } // İsim Soyisim
        public string? About { get; set; } // Hakkında
        public string? ProfileImage { get; set; } // Profil fotoğrafı url veya dosya adı
        public required string FirstName { get; set; } // İsim
        public required string LastName { get; set; } // Soyisim
        public string? AuthenticatorKey { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 