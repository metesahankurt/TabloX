using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TabloX2.Models;

namespace TabloX2.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
        public async Task<IActionResult> Register(string UserName, string FirstName, string LastName, string Password)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(string.Empty, "Tüm alanlar zorunludur.");
                return View();
            }
            var user = new ApplicationUser
            {
                UserName = UserName,
                NormalizedUserName = UserName.ToUpperInvariant(),
                FirstName = FirstName,
                LastName = LastName,
                FullName = FirstName + " " + LastName,
                DisplayUserName = UserName
            };
            var result = await _userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult RegisterConfirmation(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [AllowAnonymous]
        public IActionResult ConfirmEmail(string email, string code)
        {
            ViewBag.Email = email;
            ViewBag.Code = code;
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string UserName, string Password)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı ve şifre gereklidir.");
                return View();
            }
            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
                return View();
            }

            // Şifreyi doğrudan kontrol et
            var passwordValid = await _userManager.CheckPasswordAsync(user, Password);
            if (!passwordValid)
            {
                ModelState.AddModelError(string.Empty, "Şifre yanlış.");
                return View();
            }

            // Giriş işlemini gerçekleştir
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }
    }
} 