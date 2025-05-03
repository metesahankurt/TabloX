using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TabloX2.Models;
using TabloX2.Models.ViewModels;
using System.Text.RegularExpressions;
using TabloX2.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace TabloX2.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailService, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Kullanıcı oturumu yoksa login sayfasına yönlendir
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser 
                { 
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DisplayUserName = model.UserName,
                    PhoneNumber = model.PhoneNumber,
                    FullName = $"{model.FirstName} {model.LastName}"
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Email doğrulama token'ı oluştur
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);

                    // Email gönder
                    var emailBody = $@"
                        <h2>TabloX Hesap Doğrulama</h2>
                        <p>Merhaba {user.FirstName},</p>
                        <p>Hesabınızı doğrulamak için lütfen aşağıdaki bağlantıya tıklayın:</p>
                        <p><a href='{confirmationLink}'>Hesabımı Doğrula</a></p>
                        <p>Bu bağlantı 24 saat geçerlidir.</p>
                        <p>İyi günler,<br>TabloX Ekibi</p>";

                    await _emailService.SendEmailAsync(user.Email, "TabloX Hesap Doğrulama", emailBody);

                    TempData["SuccessMessage"] = "Kayıt başarılı! Lütfen email adresinizi kontrol edin ve hesabınızı doğrulayın.";
                    return RedirectToAction("RegisterConfirmation", new { email = user.Email });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult RegisterConfirmation(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Kullanıcı ID '{userId}' bulunamadı.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Email adresiniz başarıyla doğrulandı. Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = "Email doğrulama başarısız oldu.";
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Bu email adresiyle kayıtlı kullanıcı bulunamadı.";
                return RedirectToAction("Login");
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                TempData["InfoMessage"] = "Email adresiniz zaten doğrulanmış.";
                return RedirectToAction("Login");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token }, Request.Scheme);

            var emailBody = $@"
                <h2>TabloX Hesap Doğrulama</h2>
                <p>Merhaba {user.FirstName},</p>
                <p>Hesabınızı doğrulamak için lütfen aşağıdaki bağlantıya tıklayın:</p>
                <p><a href='{confirmationLink}'>Hesabımı Doğrula</a></p>
                <p>Bu bağlantı 24 saat geçerlidir.</p>
                <p>İyi günler,<br>TabloX Ekibi</p>";

            await _emailService.SendEmailAsync(user.Email, "TabloX Hesap Doğrulama", emailBody);

            TempData["SuccessMessage"] = "Doğrulama emaili tekrar gönderildi. Lütfen email adresinizi kontrol edin.";
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            _logger.LogInformation("Giriş denemesi başladı: {username}", model.UsernameOrEmail);

            if (ModelState.IsValid)
            {
                var isEmail = model.UsernameOrEmail.Contains("@");
                var user = isEmail 
                    ? await _userManager.FindByEmailAsync(model.UsernameOrEmail)
                    : await _userManager.FindByNameAsync(model.UsernameOrEmail);

                if (user != null)
                {
                    _logger.LogInformation("Kullanıcı bulundu: {userId}", user.Id);

                    // Önce şifreyi kontrol et
                    var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
                    
                    if (passwordCheck.Succeeded)
                    {
                        _logger.LogInformation("Şifre doğrulaması başarılı");

                        // 2FA aktif mi kontrol et
                        var is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
                        _logger.LogInformation("2FA Durumu: {status}", is2faEnabled);

                        if (is2faEnabled)
                        {
                            // Önce mevcut oturumu temizle
                            await _signInManager.SignOutAsync();

                            // 2FA için giriş denemesi yap
                            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);
                            if (signInResult.RequiresTwoFactor)
                            {
                                _logger.LogInformation("2FA gerekiyor, yönlendiriliyor");
                                
                                // Kullanıcı bilgilerini Session'a kaydet
                                HttpContext.Session.SetString("2FA_UserId", user.Id);
                                HttpContext.Session.SetString("2FA_RememberMe", model.RememberMe.ToString());
                                HttpContext.Session.SetString("2FA_ReturnUrl", returnUrl ?? string.Empty);

                                return RedirectToAction(nameof(LoginWith2fa));
                            }
                            else
                            {
                                _logger.LogWarning("2FA aktif ama RequiresTwoFactor false döndü");
                                ModelState.AddModelError(string.Empty, "2FA doğrulama hatası oluştu.");
                                return View(model);
                            }
                        }

                        // 2FA aktif değilse normal giriş yap
                        _logger.LogInformation("Normal giriş yapılıyor");
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                        
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("Giriş başarılı, yönlendiriliyor");
                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            return RedirectToAction("Index", "Home");
                        }
                        else 
                        {
                            _logger.LogWarning("PasswordSignInAsync başarısız: {status}", result.ToString());
                            ModelState.AddModelError(string.Empty, "Giriş başarısız oldu.");
                        }
                    }
                    else 
                    {
                        _logger.LogWarning("Şifre doğrulaması başarısız");
                        ModelState.AddModelError(string.Empty, "Geçersiz şifre.");
                    }
                }
                else
                {
                    _logger.LogWarning("Kullanıcı bulunamadı: {username}", model.UsernameOrEmail);
                    ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LoginWith2fa()
        {
            _logger.LogInformation("LoginWith2fa sayfası açılıyor");

            // Session'dan kullanıcı bilgilerini al
            var userId = HttpContext.Session.GetString("2FA_UserId");
            var rememberMeStr = HttpContext.Session.GetString("2FA_RememberMe");
            var returnUrl = HttpContext.Session.GetString("2FA_ReturnUrl");

            bool rememberMe = false;
            if (!string.IsNullOrEmpty(rememberMeStr))
            {
                bool.TryParse(rememberMeStr, out rememberMe);
            }

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("UserId Session'da bulunamadı");
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Kullanıcı bulunamadı: {userId}", userId);
                return NotFound($"Kullanıcı ID '{userId}' bulunamadı.");
            }

            // 2FA'nın aktif olduğunu kontrol et
            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                _logger.LogWarning("Kullanıcının 2FA'sı aktif değil: {userId}", userId);
                return RedirectToAction(nameof(Login));
            }

            _logger.LogInformation("2FA doğrulama sayfası gösteriliyor: {userId}", userId);
            return View(new VerifyTwoFactorCodeViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(VerifyTwoFactorCodeViewModel model)
        {
            _logger.LogInformation("2FA doğrulama denemesi başladı");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = HttpContext.Session.GetString("2FA_UserId");
            var rememberMeStr = HttpContext.Session.GetString("2FA_RememberMe");
            var returnUrl = HttpContext.Session.GetString("2FA_ReturnUrl");

            bool rememberMe = false;
            if (!string.IsNullOrEmpty(rememberMeStr))
            {
                bool.TryParse(rememberMeStr, out rememberMe);
            }

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("2FA doğrulama için UserId bulunamadı");
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("2FA doğrulama için kullanıcı bulunamadı: {userId}", userId);
                return NotFound($"Kullanıcı ID '{userId}' bulunamadı.");
            }

            // Doğrulama kodunu kontrol et
            var authenticatorCode = model.Code?.Replace(" ", string.Empty).Replace("-", string.Empty);
            _logger.LogInformation("2FA kodu doğrulanıyor: {userId}", userId);

            // Önce mevcut oturumu temizle
            await _signInManager.SignOutAsync();

            // 2FA doğrulaması yap
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
                authenticatorCode,
                rememberMe,
                model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("2FA doğrulama başarılı: {userId}", userId);
                
                // Başarılı girişten sonra Session'ı temizle
                HttpContext.Session.Remove("2FA_UserId");
                HttpContext.Session.Remove("2FA_RememberMe");
                HttpContext.Session.Remove("2FA_ReturnUrl");

                // Başarılı girişten sonra yönlendir
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _logger.LogWarning("2FA doğrulama başarısız: {userId}", userId);
                ModelState.AddModelError(string.Empty, "Geçersiz doğrulama kodu.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError(string.Empty, "E-posta zorunludur.");
                return View();
            }
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Hesap bulunamadı.");
                return View();
            }
            // Şifre sıfırlama token'ı oluştur
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = token }, Request.Scheme);
            var maskedUserName = MaskUserName(user.UserName);
            var emailBody = $@"
                <h2>TabloX Şifre Sıfırlama</h2>
                <p>Merhaba {maskedUserName},</p>
                <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:</p>
                <p><a href='{resetLink}'>Şifre Sıfırlama Linki</a></p>
                <p>Bu bağlantı 30 dakika geçerlidir.</p>
                <p>İyi günler,<br>TabloX Ekibi</p>";
            await _emailService.SendEmailAsync(user.Email, "TabloX Şifre Sıfırlama", emailBody);
            ViewBag.Message = "Şifre sıfırlama linki e-posta adresinize gönderildi.";
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.MaskedUserName = MaskUserName(user.UserName);
            ViewBag.UserId = userId;
            ViewBag.Code = code;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string userId, string code, string Password, string ConfirmPassword)
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ModelState.AddModelError(string.Empty, "Tüm alanlar zorunludur.");
            }
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Şifreler eşleşmiyor.");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                ViewBag.MaskedUserName = MaskUserName(user.UserName);
                ViewBag.UserId = userId;
                ViewBag.Code = code;
                return View();
            }
            var result = await _userManager.ResetPasswordAsync(user, code, Password);
            if (result.Succeeded)
            {
                ViewBag.Success = true;
                ViewBag.MaskedUserName = MaskUserName(user.UserName);
                return View();
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            ViewBag.MaskedUserName = MaskUserName(user.UserName);
            ViewBag.UserId = userId;
            ViewBag.Code = code;
            return View();
        }

        // Kullanıcı adını maskeler (ör: me******)
        private string MaskUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName) || userName.Length < 3)
                return userName;
            return userName.Substring(0, 2) + new string('*', userName.Length - 2);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmAdmin()
        {
            var admin = await _userManager.FindByNameAsync("admin");
            if (admin != null && !admin.EmailConfirmed)
            {
                admin.EmailConfirmed = true;
                await _userManager.UpdateAsync(admin);
                return Content("Admin hesabı onaylandı.");
            }
            return Content("Admin hesabı zaten onaylı veya bulunamadı.");
        }
    }
} 