using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using MailKit.Security;
using MimeKit;
using System;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace CrystalByRiya.Pages
{
    public class myaccountModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public myaccountModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Register register { get; set; }

        public string Currenturl { get; private set; }
        public MailCredentials MailCredentials { get; private set; }
        public void OnGet()
        {
            // Get the current URL
            Currenturl = HttpContext.Request.GetDisplayUrl();
        }

        // Handles registration
        public async Task<IActionResult> OnPost(string Email,string phone)
        {
            
            try
            {
                if (await _context.TblRegisters.AnyAsync(r => r.Email == Email || r.PhoneNumber == phone))
                {
                    TempData["ErrorMessage"] = "Registration failed because the email or phone number already exists.";
                    return Page();
                }
                else
                {
                    register.Email = Email;
                    register.PhoneNumber = phone;
                    await _context.TblRegisters.AddAsync(register);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Registration successful.";
                }
                MailCredentials = await _context.TblMailCredentials.Where(e=>e.Id==2).FirstOrDefaultAsync();

                // Create email
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(MailCredentials.MailId));
                email.To.Add(MailboxAddress.Parse(MailCredentials.MailId));
                email.To.Add(MailboxAddress.Parse(register.Email));
                email.Subject = "Welcome to Crystals By Riya!";

                // Construct email body
                BodyBuilder builder = new BodyBuilder
                {
                    HtmlBody = $@"
                <p>Dear {register.Name},</p>
                <p>Thank you for registering! We're thrilled to have you on board.</p>
                <p>Stay tuned for updates and exciting offers. Let’s begin this exciting journey together!</p>
                <br />
                <p>Team,</p>
                <p><strong>Crystals By Riya</strong></p>
            "
                };

                email.Body = builder.ToMessageBody();

                // Send email
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                    smtp.Authenticate(MailCredentials.MailId, MailCredentials.MailPassword);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                // Set session variables
                HttpContext.Session.SetString("UserEmail", register.Email);
                HttpContext.Session.SetString("UserName", register.Name); // Assuming Name field exists
                HttpContext.Session.SetString("Password", register.Password);
                HttpContext.Session.SetString("Phone", register.PhoneNumber);

                // Redirect based on session state
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("cart")))
                {
                    return RedirectToPage("cart");
                }
                else
                {
                    return RedirectToPage("Index");
                }
            }
            catch (Exception ex)
            {
                // Handle exception (add logging if required)
                return Page();
            }
        }


        public async Task<IActionResult> OnPostLoginAsync()
        {

            // Check if the user exists in the database with the provided email and password
            var isexist = await _context.TblRegisters
                .SingleOrDefaultAsync(e => e.Email == register.Email && e.Password == register.Password);

            if (isexist != null)
            {
                // Store login session in cookies or session
                HttpContext.Session.SetString("UserEmail", register.Email);
                HttpContext.Session.SetString("UserName", isexist.Name); // Assuming Name field exists
                HttpContext.Session.SetString("Password", register.Password);
                HttpContext.Session.SetString("Phone", isexist.PhoneNumber);


               
              
                
               
                    return RedirectToPage("/Index");
                
            }
            else
            {
                // If login fails, reload the page and display an error message
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }
        }
    }
}



