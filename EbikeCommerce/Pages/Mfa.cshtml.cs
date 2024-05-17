using EbikeCommerce.DBmodel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OtpNet;
using System.Security.Claims;

namespace EbikeCommerce.Pages
{
    [BindProperties]
    public class MfaModel : PageModel
    {
        public required string Username { get; set; }
        public required string Code { get; set; }
        public required string GoToCat { get; set; }
        public required string Message { get; set; }

        CustomerRecord? rec;

        public void OnGet(string username, string Message)
        {
            GoToCat = Message;
            Username = username;
        }

        public IActionResult OnPostVerify()
        {

            rec = DBservice.GetbyUser(Username);
            if (rec == null)
                return Page();

            if (User == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(rec.mfa))
            {
                return NotFound();
            }

            if (CompareOtpCode(rec.mfa, Code))
            {

                var claims = new List<Claim>();

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

                if (GoToCat == "You must LogIn before buy")
                {
                    return RedirectToPage("/Cart");
                }
                return RedirectToPage("/Index");
            }
            else 
            {
                Message = "Invalid code";
                return Page();
            }
        }

        public static bool CompareOtpCode(string otpSecret, string inputCode)
        {
            var otp = new Totp(Base32Encoding.ToBytes(otpSecret));
            return otp.VerifyTotp(inputCode, out _);
        }
    }
}
