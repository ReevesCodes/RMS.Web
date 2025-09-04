namespace RMS.Web.Models;
using RMS.Data.Entities;
public class PaginatedProductViewModel
{
    public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
    public int CurrentPage { get; set; } 
    public int PageSize { get; set; } 
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}
