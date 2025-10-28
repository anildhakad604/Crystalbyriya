using CrystalByRiya.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Channels;

namespace CrystalByRiya.Pages
{
    public class adminloginModel : PageModel
    {
        [BindProperty]
        public Employee Admin { get; set; }
        public ApplicationDbContext Context;
        public adminloginModel(ApplicationDbContext _Context)
        {
            Context = _Context;

        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                Admin = await Context.TblEmployee.FirstOrDefaultAsync(uname => uname.Username == Admin.Username && uname.Password == Admin.Password);
                if (Admin != null)
                {
                    HttpContext.Session.SetString("Login", Admin.Username);
                    return RedirectToPage("/Index", new { area = "Admin" });
                }
                else
                {
                    return Redirect("adminlogin");
                }


            }
            catch (Exception ex)
            {
                return Page();
            }
        }
    }
}