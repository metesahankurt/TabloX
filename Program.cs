using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using TabloX2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity yapılandırması
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // 2FA ayarları
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    options.Tokens.AuthenticatorIssuer = "TabloX";

    // 2FA için oturum ayarları
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 2FA için gerekli servisler
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(24);
});

builder.Services.AddScoped<IUserTwoFactorTokenProvider<ApplicationUser>, AuthenticatorTokenProvider<ApplicationUser>>();
builder.Services.AddScoped<IUserAuthenticatorKeyStore<ApplicationUser>, UserAuthenticatorKeyStore<ApplicationUser>>();

// Cookie ayarları
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.Cookie.Name = "TabloX.Identity";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    // 2FA için yönlendirme ayarları
    options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.Response.Redirect("/Account/Login");
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.Response.Redirect("/Account/AccessDenied");
            return Task.CompletedTask;
        },
        OnSigningIn = async context =>
        {
            // 2FA kontrolü
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(context.Principal);
            
            if (user != null)
            {
                var is2faEnabled = await userManager.GetTwoFactorEnabledAsync(user);
                if (is2faEnabled && !await signInManager.IsTwoFactorClientRememberedAsync(user))
                {
                    context.Properties.IsPersistent = false; // Geçici oturum
                }
            }
        }
    };
});

// Email ayarlarını yapılandır
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Identity'nin varsayılan route'larını tamamen devre dışı bırak
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        // Identity area'sını tamamen devre dışı bırak
        options.Conventions.AddAreaFolderRouteModelConvention("Identity", "/", model =>
        {
            foreach (var selector in model.Selectors)
            {
                selector.AttributeRouteModel = null;
            }
        });
    });

// MVC route'larını yapılandır
builder.Services.AddControllersWithViews(options =>
{
    options.EnableEndpointRouting = true;
});

// Session ayarları
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

// Session middleware'ini ekle
app.UseSession();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Veritabanını silme! Sadece migration ile oluştur.
    db.Database.Migrate();

    // Admin rolünü oluştur
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Admin kullanıcısını oluştur
    var adminUser = await userManager.FindByNameAsync("admin");
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@tablox.com",
            EmailConfirmed = true,
            DisplayUserName = "@admin",
            FirstName = "Admin",
            LastName = "User",
            FullName = "Admin User"
        };
        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Sadece hiç eser yoksa seed et
    if (!db.Artworks.Any())
    {
        await TabloX2.Data.MetMuseumSeeder.SeedAsync(db);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Önce MVC route'larını tanımla
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Identity sayfalarını engelle ve yönlendir
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Identity/Account"))
    {
        context.Response.Redirect("/Account" + context.Request.Path.Value.Substring("/Identity/Account".Length));
        return;
    }
    await next();
});

// RazorPages'i en sona al
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.Initialize(context, userManager, roleManager);
}

app.Use(async (context, next) =>
{
    var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var user = await userManager.GetUserAsync(context.User);
    if (user != null)
    {
        context.Items["CurrentUser"] = user;
    }
    await next();
});

app.Run();
