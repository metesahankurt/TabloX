using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TabloX2.Data;
using TabloX2.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using System.IO;

namespace TabloX2.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Sipariş geçmişi
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Artwork)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Artwork)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Artwork)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var subtotal = cart.Items.Sum(i => (i.Artwork?.Price ?? 0) * i.Quantity);
            var tax = subtotal * 0.18M;
            var total = subtotal + tax;

            var model = new CheckoutViewModel
            {
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Email = user.Email ?? "",
                Phone = user.PhoneNumber ?? "",
                Address = user.Address ?? "",
                City = user.City ?? "",
                District = user.District ?? "",
                PostalCode = user.PostalCode ?? "",
                PaymentMethod = "CreditCard",
                SubTotal = subtotal,
                Tax = tax,
                Total = total
            };

            ViewBag.Cart = cart;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Sadece kredi kartı validasyonu
            if (string.IsNullOrWhiteSpace(model.CardNumber) || !LuhnCheck(model.CardNumber.Replace(" ", "")))
                ModelState.AddModelError("CardNumber", "Geçerli bir kredi kartı numarası giriniz.");
            if (string.IsNullOrWhiteSpace(model.CardExpiry) || !System.Text.RegularExpressions.Regex.IsMatch(model.CardExpiry, @"^(0[1-9]|1[0-2])\/\d{2}$"))
                ModelState.AddModelError("CardExpiry", "Geçerli bir son kullanma tarihi giriniz (AA/YY).");
            if (string.IsNullOrWhiteSpace(model.CardCvv) || !System.Text.RegularExpressions.Regex.IsMatch(model.CardCvv, @"^\d{3,4}$"))
                ModelState.AddModelError("CardCvv", "Geçerli bir CVV giriniz.");
            if (string.IsNullOrWhiteSpace(model.CardHolder))
                ModelState.AddModelError("CardHolder", "Kart üzerindeki isim zorunludur.");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Artwork)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var subtotal = cart.Items.Sum(i => (i.Artwork?.Price ?? 0) * i.Quantity);
            var tax = subtotal * 0.18M;
            var total = subtotal + tax;

            var order = new Order
            {
                UserId = user.Id,
                OrderNumber = GenerateOrderNumber(),
                OrderDate = DateTime.UtcNow,
                ShippingAddress = $"{model.Address}\n{model.District}, {model.City} {model.PostalCode}",
                PhoneNumber = model.Phone,
                ShippingStatus = "Hazırlanıyor",
                Status = OrderStatus.Alindi,
                TotalAmount = total,
                DiscountAmount = 0,
                FinalAmount = total
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart.Items)
            {
                if (item.Artwork != null)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ArtworkId = item.ArtworkId,
                        Quantity = item.Quantity,
                        Price = item.Artwork.Price,
                        Order = order,
                        Artwork = item.Artwork
                    };
                    _context.OrderItems.Add(orderItem);
                }
            }

            // Sepeti temizle
            _context.CartItems.RemoveRange(cart.Items);
            cart.Items.Clear();
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Artwork)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        private string GenerateOrderNumber()
        {
            return $"TBL-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }

        // Luhn algoritması ile kredi kartı kontrolü
        private bool LuhnCheck(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(cardNumber[i].ToString());
                if (alternate)
                {
                    n *= 2;
                    if (n > 9) n -= 9;
                }
                sum += n;
                alternate = !alternate;
            }
            return (sum % 10 == 0);
        }

        [HttpGet]
        public async Task<IActionResult> Invoice(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Artwork)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
                return NotFound();

            var stream = new MemoryStream();
            var now = DateTime.Now;
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("TabloX").FontSize(24).Bold().FontColor(Colors.Red.Medium);
                            col.Item().Text($"FATURA").FontSize(16).Bold();
                        });
                        row.ConstantItem(120).Image("wwwroot/Logo/tabloXLogo.png", ImageScaling.FitArea);
                    });
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Fatura Tarihi: {now:dd.MM.yyyy HH:mm}").FontSize(10);
                        col.Item().Text($"Sipariş No: {order.OrderNumber}").FontSize(10);
                        col.Item().Text($"Müşteri: {user.FullName ?? user.UserName}").FontSize(10);
                        col.Item().Text($"Adres: {order.ShippingAddress}").FontSize(10);
                        col.Item().Text("");
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("#").Bold();
                                header.Cell().Element(CellStyle).Text("Ürün").Bold();
                                header.Cell().Element(CellStyle).Text("Adet").Bold();
                                header.Cell().Element(CellStyle).Text("Tutar").Bold();
                            });
                            int i = 1;
                            foreach (var item in order.OrderItems)
                            {
                                table.Cell().Element(CellStyle).Text(i.ToString());
                                table.Cell().Element(CellStyle).Text(item.Artwork?.Title ?? "-");
                                table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
                                table.Cell().Element(CellStyle).Text($"${item.Price * item.Quantity:N2}");
                                i++;
                            }
                        });
                        col.Item().Text("");
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("");
                            row.ConstantItem(200).Column(c2 =>
                            {
                                c2.Item().Text($"Ara Toplam: ${order.OrderItems.Sum(i => i.Price * i.Quantity):N2}");
                                c2.Item().Text($"KDV (%18): ${order.OrderItems.Sum(i => i.Price * i.Quantity) * 0.18M:N2}");
                                c2.Item().Text($"Toplam: ${order.TotalAmount:N2}").Bold();
                            });
                        });
                    });
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("TabloX - Modern Sanat Galerisi").FontSize(10);
                    });
                });
            });
            doc.GeneratePdf(stream);
            stream.Position = 0;
            return File(stream, "application/pdf", $"Fatura_{order.OrderNumber}.pdf");

            IContainer CellStyle(IContainer container) => container.PaddingVertical(4).PaddingHorizontal(2);
        }
    }
} 