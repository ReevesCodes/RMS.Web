using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService svc;

        public ProductController(IProductService productService)
        {
            svc = productService;
        }

      
        public IActionResult Index(int page = 1, int pageSize = 12)
{
    if (page < 1) page = 1;

    var total = svc.GetTotalProductsCount();
    var products = svc.GetPaginatedProducts(page, pageSize);

    var vm = new PaginatedProductViewModel
    {
        Products = products,
        CurrentPage = page,
        PageSize = pageSize,
        TotalItems = total
    };

    return View(vm);
}


        public IActionResult Details(int id)
        {
            var product = svc.GetProductById(id);
            if (product == null)
            {
                Alert("Product not found", AlertType.warning);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            svc.AddProduct(product);
            Alert("Product created successfully!", AlertType.success);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var product = svc.GetProductById(id);
            if (product == null)
            {
                Alert("Product not found", AlertType.warning);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(int id, Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            svc.UpdateProduct(product);
            Alert("Product updated", AlertType.success);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var product = svc.GetProductById(id);
            if (product == null)
            {
                Alert("Product not found", AlertType.warning);
                return RedirectToAction("Index");
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            svc.DeleteProduct(id);
            Alert("Product deleted", AlertType.success);
            return RedirectToAction("Index");
        }
    }
}


