using Microsoft.AspNetCore.Mvc;
using RMS.Data.Services;

namespace RMS.Web.Controllers
{
    public class CartController : BaseController
    {
        private readonly ICartService svc;

        public CartController(ICartService cartService)
        {
            svc = cartService;
        }

        public IActionResult Index()
        {
            var userId = GetSignedInUserId();
            var cartItems = svc.GetCartItems(userId);
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var userId = GetSignedInUserId();
            svc.AddToCart(userId, productId, quantity);
            Alert("Item added to cart", AlertType.success);
            return RedirectToAction("Index", "Shop");
        }

        public IActionResult RemoveFromCart(int id)
        {
            svc.RemoveFromCart(id);
            Alert("Item removed from cart", AlertType.info);
            return RedirectToAction("Index");
        }
    }
}


