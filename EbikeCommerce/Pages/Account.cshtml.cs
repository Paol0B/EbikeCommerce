using EbikeCommerce.DBmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System;
using OtpNet;

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
        public string? QrCode { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public IActionResult OnGet()
        {
            CustomerRecord = DBservice.GetbyUser(User.Identity?.Name)!;


            if (CustomerRecord == null)
            {
                MessageAcc = "Customer not found. Error 500 (internal server error)";
                return StatusCode(500);
            }

            if (!string.IsNullOrWhiteSpace(CustomerRecord.mfa))
            {
                TwoFactorEnabled = true;
                ShowQR();
            }




            return Page();
        }



        //qr
        public IActionResult OnPostCreateMFA()
        {
           CustomerRecord = DBservice.GetbyUser(User.Identity?.Name)!;
            if (string.IsNullOrWhiteSpace(CustomerRecord.mfa))
            {
                ShowQR();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveMfa()
        {
            CustomerRecord = DBservice.GetbyUser(User.Identity?.Name)!;
            CustomerRecord.mfa = "";
            DBservice.UpdateCustomer(CustomerRecord);
            return RedirectToPage();
        }


        public IActionResult ShowQR()
        {
            string accountName = User.Identity?.Name!;
            string issuer = "PB-Shop";
            string otpSecret = GenerateOtpSecret();
            string newotpUri = GenerateOtpUri(accountName, issuer, otpSecret);
            CustomerRecord = DBservice.GetbyUser(User.Identity?.Name)!;

            if (accountName is null)
                return Page();


            if (string.IsNullOrWhiteSpace(CustomerRecord.mfa))
            {
                QRCodeGenerator qrGenerator = new ();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(newotpUri, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new (qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);
                QrCode = "data:image/png;base64," + Convert.ToBase64String(qrCodeBytes);
                // Update the record with the new otp secret
                CustomerRecord.mfa = otpSecret;
                DBservice.UpdateCustomer(CustomerRecord);

            }
            else
            {
                TwoFactorEnabled = true;

                string otpUri = GenerateOtpUri(accountName, issuer, CustomerRecord.mfa!);



                QRCodeGenerator qrGenerator = new ();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new (qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);
                QrCode = "data:image/png;base64," + Convert.ToBase64String(qrCodeBytes);
            }

            return Page();
        }

        static string GenerateOtpSecret()
        {
            var otpSecret = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(otpSecret);
        }

        static string GenerateOtpUri(string accountName, string issuer, string otpSecret)
        {
            var otpAuthUri = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(accountName)}?secret={Uri.EscapeDataString(otpSecret)}&issuer={Uri.EscapeDataString(issuer)}";
            return otpAuthUri;
        }

        public static bool CompareOtpCode(string otpSecret, string inputCode)
        {
            var otp = new Totp(Base32Encoding.ToBytes(otpSecret));
            return otp.VerifyTotp(inputCode, out _);
        }
        //end qr

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
