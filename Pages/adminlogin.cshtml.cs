using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Pages
{
    public class adminloginModel : PageModel
    {
        [BindProperty]
        public string EmailId { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        private readonly ApplicationDbContext Context;

        public adminloginModel(ApplicationDbContext _Context)
        {
            Context = _Context;
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var admin = await Context.TblAdmin
                .FirstOrDefaultAsync(uname => uname.EmailId == EmailId && uname.Password == Password);

            if (admin != null)
            {
                HttpContext.Session.SetString("Login", admin.EmailId);
                return RedirectToPage("/Index", new { area = "Admin" });
            }

            ErrorMessage = "Invalid username or password.";
            ModelState.AddModelError(string.Empty, ErrorMessage);
            return Page();
        }
    }
}
