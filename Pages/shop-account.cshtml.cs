using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using CrystalByRiya.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CrystalByRiya.Pages
{
    public class shop_accountModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public shop_accountModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TblBillingDetail Detail { get; set; }

        public string UserEmail { get; private set; }
        public string Password { get; private set; }

        // GET: Load the page and prepopulate the email if available
        public void OnGet()
        {

            // Retrieve the session value for "UserEmail"
            UserEmail = HttpContext.Session.GetString("UserEmail");
            Password = HttpContext.Session.GetString("Password");

            // Fetch the most recent billing detail for the user based on Emailid
            if (!string.IsNullOrEmpty(UserEmail))
            {
                Detail = _context.TblBillingDetails
                                .Where(b => b.Emailid == UserEmail)
                           .FirstOrDefault();

                // If no detail is found, initialize a new one with the email
                if (Detail == null)
                {
                    Detail = new TblBillingDetail
                    {
                        Emailid = UserEmail
                    };
                }
            }
        }
    }
}
   
        

    
