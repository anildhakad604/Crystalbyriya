using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq.Expressions;
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
        { try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                await _context.TblContactUs.AddAsync(contactus);

                await _context.SaveChangesAsync();

                return RedirectToPage("/Index");
            }
            catch (Exception ex) { 
                return Page();
            }

        }

    }
}
