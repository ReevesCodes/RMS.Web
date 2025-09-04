using Microsoft.AspNetCore.Mvc;
using RMS.Data.Repository;
using RMS.Data.Services;
using RMS.Web.Models;
using RMS.Data.Entities;

namespace RMS.Web.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService svc;
        private readonly DataContext db;

        public OrderController(IOrderService orderService, DataContext context)
        {
            svc = orderService;
            db = context;
        }

        // GET: /Order/Checkout
        [HttpGet]
        public IActionResult Checkout()
        {
            var model = new CheckoutViewModel();
            return View(model);
        }

        // POST: /Order/Checkout
        [HttpPost]
        public IActionResult Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetSignedInUserId();

            // Map ViewModel to Entity objects
            var deliveryAddressEntity = new DeliveryAddress
            {
                FullName = model.DeliveryAddress.FullName,
                AddressLine1 = model.DeliveryAddress.AddressLine1,
                AddressLine2 = model.DeliveryAddress.AddressLine2,
                City = model.DeliveryAddress.City,
                PostalCode = model.DeliveryAddress.PostalCode,
                Country = model.DeliveryAddress.Country,
                PhoneNumber = model.DeliveryAddress.PhoneNumber,
                UserId = userId,
            };

            var paymentMethodEntity = new PaymentMethod
            {
                CardHolderName = model.PaymentMethod.CardHolderName,
                CardNumber = model.PaymentMethod.CardNumber,
                // ExpirationDate = model.PaymentMethod.ExpirationDate,
                CVV = model.PaymentMethod.CVV,
                UserId = userId,
            };

            // Save to database
            db.DeliveryAddresses.Add(deliveryAddressEntity);
            db.PaymentMethods.Add(paymentMethodEntity);
            db.SaveChanges();

            // Create order with saved entities' IDs
            svc.CreateOrder(userId, deliveryAddressEntity.Id, paymentMethodEntity.Id);

            Alert("Order placed successfully", AlertType.success);
            return RedirectToAction("History");
        }

        // GET: /Order/History
        public IActionResult History()
        {
            var userId = GetSignedInUserId();
            var orders = svc.GetUserOrders(userId);
            return View(orders);
        }
    }
}

