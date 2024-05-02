using EbikeCommerce.DBmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace EbikeCommerce.Pages
{
    [BindProperties]
    public class AccountModel : PageModel
    {
        public required CustomerRecord CustomerRecord { get; set; }
        public string? OldPassword { get; set; }
        public required string NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? MessagePass { get; set; }
        public string? MessageAcc { get; set; }

        public IActionResult OnGet()
        {
            CustomerRecord = DBservice.GetbyUser(User.Identity?.Name)!;

            if (CustomerRecord == null)
            {
                MessageAcc = "Customer not found. Error 500 (internal server error)";
                return StatusCode(500);
            }

            return Page();
        }

        public IActionResult OnPostAccount()
        {
            try
            {
                DBservice.UpdateCustomer(CustomerRecord);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                Console.WriteLine($"An error occurred while updating customer: {ex.Message}");
                MessageAcc = "An error occurred while processing your request.";
                return Page();
            }
        }

        public IActionResult OnPostChangePassword()
        {
            var customerRecord = DBservice.GetbyUser(User.Identity?.Name);

            if (customerRecord == null)
            {
                MessagePass = "Customer not found.";
                return Page();
            }

            if (!BCrypt.Net.BCrypt.Verify(OldPassword, customerRecord.passwd))
            {
                MessagePass = "Old password is incorrect.";
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                MessagePass = "Passwords do not match.";
                return Page();
            }

            try
            {
                DBservice.UpdatePassword(customerRecord.customer_id, NewPassword);
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                Console.WriteLine($"An error occurred while updating password: {ex.Message}");
                MessagePass = "An error occurred while processing your request.";
                return Page();
            }
        }
    }
}
