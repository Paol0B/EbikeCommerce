using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using EbikeCommerce.DBmodel;

namespace EbikeCommerce.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public required string Username { get; set; }

        [BindProperty]
        public required string Password { get; set; }

        [BindProperty]
        public required string Message { get; set; }

        public void OnGet(string? message)
        {
            Message = message ?? string.Empty;
        }

        public IActionResult OnPost()
        {
            return Page();
        }

        public IActionResult OnPostLogin()
        {
            var claims = new List<Claim>();

            try
            {
                if (!DBservice.CheckLogin(Password, Username))
                {
                    Message = "Invalid login attempt.";
                    return Page();
                }

                Username = !string.IsNullOrEmpty(DBservice.FindUserByEmail(Username)) ? DBservice.FindUserByEmail(Username) : Username;


                //Setting  
                claims.Add(new Claim(ClaimTypes.Name, Username));
                //claims.Add(new Claim(ClaimTypes.Role, Credential.Username));

                var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimPrincipal = new ClaimsPrincipal(claimIdenties);

                //var authenticationProperties = new AuthenticationProperties() { IsPersistent = isPersistent };

                var authenticationManager = Request.HttpContext;

                // Sign In.  
                authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);

                if (Message == "You must LogIn before buy")
                {
                    return RedirectToPage("/Cart");
                }

                return RedirectToPage("/index");
            }
            catch (Exception)
            {
                return Page();
            }
        }

        public IActionResult OnGetLogout()
        {
            var authenticationManager = Request.HttpContext;

            // Sign Out.  
            authenticationManager.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return this.RedirectToPage("/Index");
        }
    }
}
