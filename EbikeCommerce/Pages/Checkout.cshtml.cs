using EbikeCommerce.DBmodel;
using EbikeCommerce.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Numerics;

namespace EbikeCommerce.Pages
{
    [BindProperties]
    public class CheckoutModel : PageModel
    {
        public required Orders Orders { get; set; }

        public required CustomerRecord Customer { get; set; }

        public required CreditCard card { get; set; }

        public required string Message { get; set; }

        public void OnGet()
        {
            List<int>? ids = HttpContext.Session.GetObject<List<int>>($"Carrello-{User?.Identity?.Name}");

            if (ids is null)
            {
                HttpContext.Response.StatusCode = 500;
                Message = "error 500 (internal server error)";
                return;
            }

            Customer = DBservice.GetbyUser(User.Identity?.Name)!;
        }

        public IActionResult OnPostUpdateAddress() 
        {
            DBservice.UpdateCustomer(Customer);
            return Page();
        }

        public IActionResult OnPostBuy()
        {
            if (User.Identity?.Name == null || card == null)
                return Page();

            List<int> cart = HttpContext.Session.GetObject<List<int>>($"Carrello-{User?.Identity?.Name}") ?? [];

            if (DBservice.Buy(user: User.Identity.Name, cart))
            {
                HttpContext.Session.Remove($"Carrello-{User?.Identity?.Name}");

                return RedirectToPage("/Index");
            }
            else
            {
                Message = "Error during the purchase";
                return Page();
            }
        }
    }
}
