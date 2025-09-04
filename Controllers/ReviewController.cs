
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Services;
using System.Security.Claims;

namespace RMS.Web.Controllers
{
    public class ReviewController : BaseController
    {
        private readonly IReviewService svc;

        public ReviewController(IReviewService reviewService)
        {
            svc = reviewService;
        }

        // GET: /Review/Create/{productId}
        [HttpGet]
        public IActionResult Create(int productId)
        {
            var review = new Review { ProductId = productId };
            return View(review);
        }

        // POST: /Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Review review)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    // Logged-in user
                    review.UserId = int.Parse(userIdClaim.Value);
                }
                else
                {
                    // Guest user
                    review.UserId = null;
                    if (string.IsNullOrWhiteSpace(review.GuestName))
                    {
                        review.GuestName = "Guest";
                    }
                }

                svc.AddReview(review);
                Alert("Review submitted successfully.", AlertType.success);
                return RedirectToAction("Details", "Product", new { id = review.ProductId });
            }

            return View(review);
        }

        // POST: /Review/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var review = svc.GetReviewById(id);

            if (review == null)
            {
                Alert("Review not found.", AlertType.warning);
                return NotFound();
            }

            // Check authorization: Admin OR review owner
            if (!User.IsInRole("Admin") && review.UserId != GetCurrentUserId())
            {
                Alert("You are not authorized to delete this review.", AlertType.danger);
                return RedirectToAction("Details", "Product", new { id = review.ProductId });
            }

            svc.DeleteReview(id);
            Alert("Review deleted successfully.", AlertType.success);

            return RedirectToAction("Details", "Product", new { id = review.ProductId });
        }

        // Helper method to get current logged-in user id
        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : (int?)null;
        }
    }
}
