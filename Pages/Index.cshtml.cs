using CrystalByRiya.Models;
using CrystalByRiya.StoredProcedure;
using CrystalByRiya.@class;
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
        public List<TblCategory> AllCategories { get; set; }

        public List<IntentionMaster> IntentionsItem { get; set; }
        // Contact Information for Consultation Section
        public string ContactPhone { get; set; } = "+123456789";
        public string ContactWhatsApp { get; set; } = "123456789";
        public string ConsultationTitle { get; set; } = "Not Sure Which Product to Buy?";
        public string ConsultationSubText1 { get; set; } = "Choosing the right gemstone, crystal, or spiritual product can feel overwhelming. Every person's journey is unique — and what works for someone else may not be the perfect match for you.";
        public string ConsultationSubText2 { get; set; } = "Our team of experienced Astro Experts will guide you based on your personal needs, lifestyle, and astrological recommendations. Whether it's for healing, positivity, or prosperity — we'll make sure you get the right product for the right purpose.";
        public string ConsultationSubText3 { get; set; } = "Still confused? Don't worry! Simply reach out to us below — and we'll help you find the perfect solution.";

        private readonly AddToCartItems _addToCartItems;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context, AddToCartItems addToCartItems)
        {
            _logger = logger;
            _context = context;
            _addToCartItems = addToCartItems;
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
                    .Select(g => g.First())  // Select the first product in each grou\]
                    
                    .ToList();

                Banners = await _context.TblBanners.OrderByDescending(id => id.Id).ToListAsync();
                Instagrams = await _context.TblInstagram.OrderByDescending(e => e.Id).ToListAsync();
                Blogs = await _context.TblBlogs.Where(e => e.IsActive == true).OrderByDescending(e => e.Blogid).Take(4).ToListAsync();
                Announcements = await _context.TblAnnouncement.ToListAsync();
                SpBestseller = await _context.SpBestSeller.FromSqlRaw("SpBestseller").ToListAsync();
                Products= await _context.TblProducts.ToListAsync();
                AllCategories = await _context.TblCategory.ToListAsync();
                IntentionsItem=await _context.TblIntentionMaster.ToListAsync();

                //Products = await _context.TblProducts
                //                        .OrderByDescending(p => p.ID)   
                //                        .ToListAsync();

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

                // Redirect to cart page after successful registration
                return RedirectToPage("/cart");
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
                    string redirectUrl = Url.Page("/Index") ?? "/Index";
                    return RedirectToPage("/myaccount", new { redirectUrl });
                }
                
                // Direct database insertion for wishlist using correct model
                var wishlistItem = new AddingWishlist
                {
                    UserEmail = UserEmail,
                    skucode = SkuCode, // Note: property is skucode (lowercase)
                    ProductName = ProductName,
                    Date = DateTime.Now
                };
                
                await _context.TblWishlist.AddAsync(wishlistItem);
                await _context.SaveChangesAsync();

                // If 'buynow' is false, redirect to wishlist page
                return RedirectToPage("/wishlist");
            }
            catch (Exception ex)
            {
                // Log exception and return a generic error message
                ModelState.AddModelError(string.Empty, "An error occurred while adding product to wishlist.");
                return RedirectToPage("/Index");
            }
        }
        
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAddToCart(string SkuCode, int Qty, string Price, string Childsku, string AddOn, bool buynow = false)
        {
            try
            {
                var useremail = HttpContext.Session.GetString("UserEmail");
                var username = HttpContext.Session.GetString("UserName");
                var phone = HttpContext.Session.GetString("Phone");
                
                if (string.IsNullOrEmpty(useremail) && buynow == false)
                {
                    string redirectUrl = Url.Page("/Index") ?? "/Index";
                    HttpContext.Session.SetString("PendingCartSkuCode", SkuCode);
                    HttpContext.Session.SetString("PendingCartQty", Qty.ToString());
                    HttpContext.Session.SetString("PendingCartPrice", Price);
                    HttpContext.Session.SetString("PendingCartChildSku", Childsku);
                    HttpContext.Session.SetString("PendingCartAddOn", AddOn ?? string.Empty);
                    HttpContext.Session.SetString("PendingCartBuyNow", buynow.ToString());
                    return RedirectToPage("/myaccount", new { redirectUrl });
                }
                
                // Use injected AddToCartItems service
                await _addToCartItems.OnPostAddToCarts(Qty, "", "", SkuCode, buynow, Price, Childsku, AddOn, useremail, phone, username);
                
                if (buynow == true)
                {
                    return RedirectToPage("/checkout");
                }
                
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding product to cart.");
                ModelState.AddModelError(string.Empty, "An error occurred while adding product to cart.");
                return Page();
            }
        }
    }
}
