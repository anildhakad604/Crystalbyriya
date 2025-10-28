using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using MailKit.Security;
using CrystalByRiya.Models;
using System;
using Microsoft.EntityFrameworkCore;

public class ForgetPasswordModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ForgetPasswordModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Email { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPost()
    {
        var user = await _context.TblRegisters.FirstOrDefaultAsync(u => u.Email == Email);
        if (user != null)
        {
            // Generate Token
            var token = Guid.NewGuid();
            var expiryTime = DateTime.Now.AddHours(1);

            // Save Token to Database
            var resetToken = new PasswordResetToken
            {
                UserEmail = Email,
                ResetToken = token,
                ExpiryTime = expiryTime
            };
            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            // Generate Reset Link
            var resetLink = Url.Page(
                "/ResetPassword",
                null,
                new { token = token },
                Request.Scheme);

            // Send Email
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("Info@crystalsbyriya.com"));
                email.To.Add(MailboxAddress.Parse(Email));
                email.Subject = "Password Reset Request";
                var builder = new BodyBuilder
                {
                    HtmlBody = $"<p>Click the link below to reset your password. The link will expire in 1 hour:</p>" +
                               $"<a href='{resetLink}'>Reset Password</a>"
                };
                email.Body = builder.ToMessageBody();

                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                    smtp.Authenticate("Info@crystalsbyriya.com", "iilq qomm ojyw zgdp");
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                TempData["SuccessMessage"] = "Password reset link has been sent to your email.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to send the reset link. Please try again.";
            }
        }
        else
        {
            TempData["ErrorMessage"] = "No account found with this email.";
        }

        return Page();
    }
}
