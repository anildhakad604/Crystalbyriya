using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Add this
using BCrypt.Net; // Add this for BCrypt hashing
using System;
using CrystalByRiya.Models;
using Org.BouncyCastle.Crypto.Generators;

namespace CrystalByRiya.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ResetPasswordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string NewPassword { get; set; }
        public Guid Token { get; set; }

        public async Task<IActionResult> OnGet(Guid token)
        {
            var record = await _context.PasswordResetTokens.FirstOrDefaultAsync(r => r.ResetToken == token);
            if (record == null || record.ExpiryTime < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Invalid or expired reset link.";
                return RedirectToPage("/ForgetPassword");
            }

            Token = token;
            return Page();
        }

        public async Task<IActionResult> OnPost(Guid token)
        {
            var record = await _context.PasswordResetTokens.FirstOrDefaultAsync(r => r.ResetToken == token);
            if (record == null || record.ExpiryTime < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Invalid or expired reset link.";
                return RedirectToPage("/ForgetPassword");
            }

            var user = await _context.TblRegisters.FirstOrDefaultAsync(u => u.Email == record.UserEmail);
            if (user != null)
            {
                user.Password = NewPassword;
                 _context.TblRegisters.Update(user);
                await _context.SaveChangesAsync();

                // Remove token from database
                _context.PasswordResetTokens.Remove(record);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Password has been reset successfully.";
                return RedirectToPage("myaccount");
            }

            TempData["ErrorMessage"] = "Failed to reset password. Please try again.";
            return RedirectToPage("/ForgetPassword");
        }
    }
}
