using CrystalByRiya.@class;
using CrystalByRiya.Models;
using CrystalByRiya.StoredProcedure;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MailKit.Security;
using MimeKit;

namespace CrystalByRiya.Pages
{
    public class IndexModel : PageModel
    {
        ApplicationDbContext _context;
        private readonly AddToWishlistModel _addToWishlist;

        private readonly ILogger<IndexModel> _logger;
        public List<SPCategoryWiseProduct> ByCategory { get; set; }
        [BindProperty]
        public CrystalByRiya.Models.Register register { get; set; }

        public List<Banner> Banners { get; set; }
        public List<Instagram> Instagrams { get; set; }
        public List<Blogs> Blogs { get; set; }
        public string Currenturl { get; set; }
        public List<Announcement> Announcements { get; set; }

        public List<Product> Products { get; set; }
        public List<BestSeller> SpBestseller { get; set; }
 


        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
      
        public async Task<IActionResult> OnGet()
        {
           
            try
            {
                Currenturl = HttpContext.Request.GetDisplayUrl();
                var cat = new SqlParameter("@CategoryId", 1);
                /*  ByCategory = await _context.SPCategoryWiseProduct.FromSqlRaw("SPCategoryWiseProduct @CategoryId", cat).ToListAsync();*/
                var allProducts = await _context.SPCategoryWiseProduct.FromSqlRaw("SPCategoryWiseProduct @CategoryId", cat).ToListAsync();

                var skuCodes = new[] { "AM003BR", "TE104RISL", "GQ045TU", "RJ084PESL" };

                // Filter the products where SkuCode matches the specified values, and ensure uniqueness
                ByCategory = allProducts
                    .Where(p => skuCodes.Contains(p.SkuCode))
                    .GroupBy(p => p.SkuCode) // Group by SkuCode to ensure no duplicates
                    .Select(g => g.First())  // Select the first product in each group
                    .ToList();

                Banners = await _context.TblBanners.OrderByDescending(id => id.Id).ToListAsync();
                Instagrams = await _context.TblInstagram.OrderByDescending(e => e.Id).ToListAsync();
                Blogs = await _context.TblBlogs.Where(e => e.IsActive == true).OrderByDescending(e => e.Blogid).Take(3).ToListAsync();
                Announcements = await _context.TblAnnouncement.ToListAsync();
                SpBestseller = await _context.SpBestSeller.FromSqlRaw("SpBestseller").ToListAsync();
                Products= await _context.TblProducts.ToListAsync();
            }
            catch (Exception ex)
            {
            }
            return Page();

        }
        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();  // Clear the session
            return RedirectToPage("/Index");  // Redirect back to index page
        }
        public async Task<IActionResult> OnPostRegister(string Email,string phone)
        {
            try
            {
                register.Email = Email;
                register.PhoneNumber = phone;
                await _context.TblRegisters.AddAsync(register);
                await _context.SaveChangesAsync();

                // Create email
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("Info@crystalsbyriya.com"));
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
                    smtp.Authenticate("Info@crystalsbyriya.com", "iilq qomm ojyw zgdp");
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
        public async Task<IActionResult> OnPostAddToWishlistAsync(string SkuCode, string ProductName, bool buynow)
        {
            try
            {

                string UserEmail = HttpContext.Session.GetString("UserEmail");

                if (string.IsNullOrEmpty(UserEmail))
                {
                    // User is not logged in, redirect to login page
                    string redirectUrl = Url.Page("/Index"); // After login, redirect to the wishlist
                    return RedirectToPage("/myaccount", new { redirectUrl });
                }
                // Use the AddToWishlist service method to add the product to the wishlist
                var result = await _addToWishlist.OnPostAddToWishlist(SkuCode, ProductName);

                // Check if the product was successfully added to the wishlist
                if (result is BadRequestResult)
                {
                    ModelState.AddModelError(string.Empty, "Failed to add product to wishlist.");
                    return RedirectToPage("/Index");
                }





                // If 'buynow' is false, redirect to the wishlist page
                return RedirectToPage("/blogdetail");
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product to the wishlist.");
                return RedirectToPage("/blogdetail");
            }
        }
    }

}



