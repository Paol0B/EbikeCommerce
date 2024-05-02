using EbikeCommerce.DBmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace EbikeCommerce.Pages
{

    public class IndexModel : PageModel
    {
        public required List<ProductRecord> ProductRecords { get; set; }

        [BindProperty]
        public string? SearchText { get; set; }
        [BindProperty]
        public int FilterYear { get; set; }
        [BindProperty]
        public int FilterPrice { get; set; }
        [BindProperty]
        public int FilterBrand { get; set; }



        public void OnGet() => ProductRecords = DBservice.OnGetDataAsync().Result;

        //search
        public IActionResult OnPostSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return RedirectToPage();

            ProductRecords = DBservice.GetFilteredData(SearchText);
            return Page();
        }

        //search with filter
        
        public IActionResult OnPostFilter() 
        {
            if (FilterYear == 0 && FilterPrice == 0 && FilterBrand == 0)
                return RedirectToPage();

            ProductRecords = DBservice.GetFilteredData(FilterYear, FilterPrice, FilterBrand);
            return Page();
        }

        public IActionResult OnPostProductPage(int id) => RedirectToPage("/ProductPage", new { id });

    }
}