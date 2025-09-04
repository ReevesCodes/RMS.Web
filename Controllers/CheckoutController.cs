using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMS.Data.Entities;
using RMS.Data.Repository;
using RMS.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace RMS.Web.Controllers
{
    public class CheckoutController : BaseController
    {
        private readonly DataContext db;

        public CheckoutController(DataContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = GetSignedInUserId();

            var cartItems = db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            var totalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);

            var model = new CheckoutViewModel
            {
                CartTotalAmount = totalAmount
            };

            ViewBag.CartTotal = totalAmount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CheckoutViewModel model)
        {
            var userId = GetSignedInUserId();

            var cartItems = db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            model.CartTotalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);

            if (!cartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CartTotal = model.CartTotalAmount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                return View(model);
            }

            // Save delivery address
            var deliveryAddressEntity = new DeliveryAddress
            {
                FullName = model.DeliveryAddress.FullName,
                AddressLine1 = model.DeliveryAddress.AddressLine1,
                AddressLine2 = model.DeliveryAddress.AddressLine2,
                City = model.DeliveryAddress.City,
                PostalCode = model.DeliveryAddress.PostalCode,
                Country = model.DeliveryAddress.Country,
                PhoneNumber = model.DeliveryAddress.PhoneNumber,
                UserId = userId
            };
            db.DeliveryAddresses.Add(deliveryAddressEntity);

            // Save payment method (for card/manual payments)
            var paymentMethodEntity = new PaymentMethod
            {
                CardHolderName = model.PaymentMethod.CardHolderName,
                CardNumber = model.PaymentMethod.CardNumber,
                ExpirationDate = model.PaymentMethod.ExpirationDate,
                UserId = userId
            };
            db.PaymentMethods.Add(paymentMethodEntity);
            db.SaveChanges();

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                DeliveryAddressId = deliveryAddressEntity.Id,
                PaymentMethodId = paymentMethodEntity.Id,
                Items = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PriceAtPurchase = ci.Product.Price
                }).ToList()
            };
            order.TotalAmount = order.Items.Sum(i => i.Quantity * i.PriceAtPurchase);

            db.Orders.Add(order);
            db.CartItems.RemoveRange(cartItems);
            db.SaveChanges();

            return RedirectToAction("ThankYou", new { orderId = order.Id });
        }

        // === PAYPAL ORDER CREATION ===
        [HttpPost]
        public async Task<IActionResult> CreatePayPalOrder()
        {
            var userId = GetSignedInUserId();
            var cartItems = db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            if (!cartItems.Any())
                return BadRequest("Your cart is empty.");

            var total = cartItems.Sum(c => c.Product.Price * c.Quantity);

            var orderRequest = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = "USD",
                            value = total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                        },
                        items = cartItems.Select(c => new
                        {
                            name = c.Product.Name,
                            quantity = c.Quantity.ToString(),
                            unit_amount = new
                            {
                                currency_code = "USD",
                                value = c.Product.Price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                            }
                        }).ToArray()
                    }
                },
                application_context = new
                {
                    return_url = Url.Action("ThankYou", "Checkout", null, Request.Scheme),
                    cancel_url = Url.Action("Index", "Checkout", null, Request.Scheme)
                }
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await GetPayPalAccessToken());

            var json = JsonConvert.SerializeObject(orderRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api-m.sandbox.paypal.com/v2/checkout/orders", content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest(result);

            return Content(result, "application/json");
        }

        // === PAYPAL CAPTURE ===
        [HttpPost]
        public async Task<IActionResult> PaypalCapture([FromBody] PayPalCaptureRequest request)
        {
            if (string.IsNullOrEmpty(request.OrderId))
                return BadRequest("Invalid PayPal order ID.");

            bool paymentVerified = await VerifyPayPalPaymentAsync(request.OrderId);
            if (!paymentVerified)
                return BadRequest("PayPal payment verification failed.");

            var userId = GetSignedInUserId();

            // Ideally, delivery address and payment method should be saved by the user before PayPal checkout or retrieved differently
            var deliveryAddressEntity = db.DeliveryAddresses.Where(d => d.UserId == userId).OrderByDescending(d => d.Id).FirstOrDefault();
            var paymentMethodEntity = db.PaymentMethods.Where(p => p.UserId == userId).OrderByDescending(p => p.Id).FirstOrDefault();

            var cartItems = db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            if (!cartItems.Any())
                return BadRequest("Cart is empty.");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                DeliveryAddressId = deliveryAddressEntity?.Id ?? 0,
                PaymentMethodId = paymentMethodEntity?.Id ?? 0,
                Items = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PriceAtPurchase = ci.Product.Price
                }).ToList()
            };
            order.TotalAmount = order.Items.Sum(i => i.Quantity * i.PriceAtPurchase);

            db.Orders.Add(order);
            db.CartItems.RemoveRange(cartItems);
            await db.SaveChangesAsync();

            return Ok(new { message = "Payment captured and order saved successfully.", orderId = order.Id });
        }

        private async Task<string> GetPayPalAccessToken()
        {
            var clientId = "YOUR_PAYPAL_CLIENT_ID";
            var secret = "YOUR_PAYPAL_SECRET";

            using var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes($"{clientId}:{secret}");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token", content);
            var result = await response.Content.ReadAsStringAsync();

            dynamic json = JsonConvert.DeserializeObject(result);
            return json.access_token;
        }

        private async Task<bool> VerifyPayPalPaymentAsync(string orderId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await GetPayPalAccessToken());

            var response = await client.GetAsync($"https://api-m.sandbox.paypal.com/v2/checkout/orders/{orderId}");
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return false;

            dynamic json = JsonConvert.DeserializeObject(result);
            return json.status == "COMPLETED";
        }

        public IActionResult ThankYou(int orderId)
        {
            var order = db.Orders
                .Include(o => o.DeliveryAddress)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null) return NotFound();

            var vm = new ThankYouViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                DeliveryAddress = order.DeliveryAddress,
                Items = order.Items.Select(i => new OrderItemViewModel
                {
                    Name = i.Product.Name,
                    Quantity = i.Quantity,
                    PriceAtPurchase = i.PriceAtPurchase
                }).ToList()
            };

            return View(vm);
        }
    }

    public class PayPalCaptureRequest
    {
        public string OrderId { get; set; }
    }
}
