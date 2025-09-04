
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Services;
using RMS.Web.Models;
using RMS.Web.Services;
using System.Linq;
using System.Collections.Generic;

namespace RMS.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductService _productService;
        private readonly ProductRecommendationService _recommendationService;
        private readonly ICategoryService categoryService;

        public ShopController(
            IProductService productService,
            ProductRecommendationService recommendationService,
            ICategoryService categoryService)
        {
            _productService = productService;
            _recommendationService = recommendationService;
            this.categoryService = categoryService;
        }

        // GET: /Shop/Index?page=1
        public IActionResult Index(int page = 1)
        {
            int pageSize = 30; // Show 20 products per page (adjust as needed)

            var products = _productService.GetAllProducts().ToList();
            var categories = categoryService.GetAllCategories().ToList();

            uint currentUserId = GetCurrentUserId();

            // Get top 8 recommended products for current user
            var recommendedProducts = products
                .OrderByDescending(p => _recommendationService.PredictScore(currentUserId, (uint)p.Id))
                .Take(8)
                .ToList();

            // Paginate remaining products
            var paginatedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalPages = (int)System.Math.Ceiling((double)products.Count / pageSize);

            var model = new ShopViewModel
            {
                Products = paginatedProducts,
                RecommendedProducts = recommendedProducts,
                Categories = categories,
                CurrentPage = page,
                TotalPages = totalPages
            };

         

            return View(model);
        }

        // GET: /Shop/Search?query=...
        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction("Index");

            var allProducts = _productService.GetAllProducts().ToList();

            var filtered = allProducts
                .Where(p => p.Name.Contains(query, System.StringComparison.OrdinalIgnoreCase) ||
                            p.Description.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            var model = new ShopViewModel
            {
                Products = filtered,
                RecommendedProducts = new List<Product>(),
                Categories = categoryService.GetAllCategories().ToList()
            };

            ViewBag.SearchTerm = query;
            return View("Index", model);
        }

        // GET: /Shop/Details/{id}
        public IActionResult Details(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpGet]
        public IActionResult Category(int id)
        {
            var category = categoryService.GetCategoryById(id);
            if (category == null)
                return NotFound();

            var products = _productService.GetProductsByCategories(new List<string> { category.Name }).ToList();
            var categories = categoryService.GetAllCategories().ToList();

            var model = new ShopViewModel
            {
                Products = products,
                RecommendedProducts = new List<Product>(),
                Categories = categories,
                CurrentCategory = category
            };

            ViewBag.CategoryName = category.Name;
            ViewBag.ProductCount = products.Count;

            return View("Index", model);
        }

        // Placeholder for getting logged-in user id as uint
        private uint GetCurrentUserId()
        {
            return 1u; // Replace with actual authentication logic
        }
    }
}




