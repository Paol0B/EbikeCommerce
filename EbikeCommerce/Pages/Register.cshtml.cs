using EbikeCommerce.DBmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeCommerce.Pages
{
    [BindProperties]
    public class RegisterModel : PageModel
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public string? Message { get; set; }

        public CustomerRecord? Customer { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            if (Password != ConfirmPassword)
            {
                Message = "Passwords do not match.";
                return Page();
            }

            Customer = new CustomerRecord
            {
                first_name = FirstName,
                last_name = LastName,
                username = Username,
                email = Email,
                passwd = Password
            };

            try
            {
                DBservice.AddCustomer(Customer);
            }
            catch (Exception)
            {
                Message = "An error occurred while processing your request. Your Username or Email already exist";
                return Page();
            }


            return RedirectToPage("/Login");
        }
    }
}
