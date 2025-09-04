using RMS.Data.Entities;
using System.Collections.Generic;

namespace RMS.Web.Models
{
    public class ShopViewModel
    {
         public List<Product> Products { get; set; }
        public List<Product> RecommendedProducts { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>(); /// i added this need be to take out
        public Category CurrentCategory { get; set; }

         public PaginatedProductViewModel PaginatedProducts { get; set; } = new PaginatedProductViewModel();

   public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;

    }
}
