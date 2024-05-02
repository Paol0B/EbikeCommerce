using EbikeCommerce.DBmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeCommerce.Pages
{
    public class ProductPageModel : PageModel
    {
        public ProductRecord? Rec { get; set; }

        public void OnGet(int id)
        {
            Rec = DBservice.GetbyID(id) ?? new ProductRecord
            {
                product_name = "Product not found"
            };
        }

        public IActionResult OnPostBuy(int id) => RedirectToPage("/Cart", new { id });
    }
}