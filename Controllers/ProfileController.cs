using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TabloX2.Models;
using TabloX2.Models.ViewModels;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using TabloX2.Helpers;
using System.Security.Cryptography;

namespace TabloX2.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ProfileController> _logger;
        private readonly IUserAuthenticatorKeyStore<ApplicationUser> _keyStore;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ProfileController> logger,
            IUserAuthenticatorKeyStore<ApplicationUser> keyStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _keyStore = keyStore;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DisplayUserName = model.DisplayUserName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(nameof(Index), user);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(nameof(Index), user);
        }

        public async Task<IActionResult> Enable2FA()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogError("Kullanıcı bulunamadı");
                    return NotFound();
                }

                // 2FA zaten aktif mi kontrol et
                if (await _userManager.GetTwoFactorEnabledAsync(user))
                {
                    return RedirectToAction(nameof(Index));
                }

                // Mevcut anahtarı kontrol et
                var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                
                if (string.IsNullOrEmpty(unformattedKey))
                {
                    // Yeni anahtar oluştur ve hemen kaydet
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                    unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                    
                    if (string.IsNullOrEmpty(unformattedKey))
                    {
                        _logger.LogError("Anahtar oluşturma başarısız oldu");
                        TempData["ErrorMessage"] = "2FA etkinleştirme sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                var model = new TwoFactorAuthenticationViewModel
                {
                    SharedKey = FormatKey(unformattedKey),
                    AuthenticatorUri = GenerateQrCodeUri(user.Email ?? user.UserName ?? "unknown", unformattedKey),
                    Is2faEnabled = false
                };

                // Oturumu yenile
                await _signInManager.RefreshSignInAsync(user);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Enable2FA'da hata: {ex.Message}\nStack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "2FA etkinleştirme sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Enable2FA(TwoFactorAuthenticationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // 2FA zaten aktif mi kontrol et
            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return RedirectToAction(nameof(Index));
            }

            // Doğrulama kodunu kontrol et
            var verificationCode = model.Code?.Replace(" ", string.Empty).Replace("-", string.Empty) ?? string.Empty;
            if (string.IsNullOrEmpty(verificationCode))
            {
                ModelState.AddModelError("Code", "Doğrulama kodu gereklidir.");
                return View(model);
            }

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Code", "Doğrulama kodu geçersiz.");
                var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                if (!string.IsNullOrEmpty(unformattedKey))
                {
                    model.SharedKey = FormatKey(unformattedKey);
                    model.AuthenticatorUri = GenerateQrCodeUri(user.Email ?? user.UserName ?? "unknown", unformattedKey);
                }
                model.Is2faEnabled = false;
                return View(model);
            }

            // 2FA'yı etkinleştir
            await _userManager.SetTwoFactorEnabledAsync(user, true);

            // Kurtarma kodlarını oluştur
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            // Güvenlik damgasını güncelle
            await _userManager.UpdateSecurityStampAsync(user);

            // Oturumu yenile
            await _signInManager.RefreshSignInAsync(user);

            _logger.LogInformation($"Kullanıcı {user.UserName} için 2FA etkinleştirildi.");

            model.RecoveryCodes = recoveryCodes?.ToArray();
            model.Is2faEnabled = true;

            TempData["SuccessMessage"] = "İki faktörlü kimlik doğrulama başarıyla etkinleştirildi.";
            return View(model);
        }

        private string FormatKey(string unformattedKey)
        {
            if (string.IsNullOrEmpty(unformattedKey))
            {
                throw new ArgumentException("Anahtar boş olamaz.", nameof(unformattedKey));
            }

            var result = new StringBuilder();
            int currentPosition = 0;

            while (currentPosition + 4 <= unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().Trim().ToUpperInvariant();
        }

        private static string Base32Encode(byte[] data)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var bits = 0;
            var value = 0;
            var output = new StringBuilder();

            for (var i = 0; i < data.Length; i++)
            {
                value = (value << 8) | data[i];
                bits += 8;
                while (bits >= 5)
                {
                    output.Append(alphabet[(value >> (bits - 5)) & 31]);
                    bits -= 5;
                }
            }

            if (bits > 0)
            {
                output.Append(alphabet[(value << (5 - bits)) & 31]);
            }

            return output.ToString();
        }

        [HttpPost]
        public async Task<IActionResult> Disable2FA()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "2FA devre dışı bırakılırken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "2FA başarıyla devre dışı bırakıldı.";
            return RedirectToAction(nameof(Index));
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email boş olamaz.", nameof(email));
            }

            if (string.IsNullOrEmpty(unformattedKey))
            {
                throw new ArgumentException("Anahtar boş olamaz.", nameof(unformattedKey));
            }

            const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
            return string.Format(
                AuthenticatorUriFormat,
                Uri.EscapeDataString("TabloX"),
                Uri.EscapeDataString(email),
                unformattedKey);
        }
    }
} 