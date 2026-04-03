using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CrystalByRiya.Pages
{
    public class contactusModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Constructor: Correctly assigning the parameter to the private field
        public contactusModel(ApplicationDbContext context)
        {
            _context = context; // Corrected this line
        }

        public string Currenturl { get; private set; }

        [BindProperty]
        public ContactUs contactus { get; set; }

        public void OnGet()
        {
            Currenturl = HttpContext.Request.GetDisplayUrl();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                // Set creation date
                contactus.Id = 0; // Let database generate ID

                await _context.TblContactUs.AddAsync(contactus);
                await _context.SaveChangesAsync();

                // Add success message
                TempData["SuccessMessage"] = "Thank you for contacting us! We will get back to you soon.";

                // Clear form fields
                contactus = new ContactUs();

                return Page();
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError("", "Database error: Unable to save your message. Please try again.");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while sending your message. Please try again.");
                return Page();
            }
        }

    }
}
