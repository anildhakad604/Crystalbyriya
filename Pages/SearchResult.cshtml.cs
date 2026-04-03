using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CrystalByRiya.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.@class;
using Microsoft.AspNetCore.Http.Extensions;

namespace CrystalByRiya.Pages
{
    public class SearchResultModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly AddToWishlistModel _addToWishlist;
        private readonly ILogger<productlistModel> _logger;
        private readonly AddToCartItems _addToCartItems;

        public SearchResultModel(ApplicationDbContext context, AddToWishlistModel addToWishlist, ILogger<productlistModel> logger, AddToCartItems addToCartItems)
        {
            _context = context;
            _addToWishlist = addToWishlist;
            _logger = logger;
            _addToCartItems = addToCartItems;
        }
        public string Currenturl { get; private set; }
        // Property to hold the selected product details
        [BindProperty]
        public List<Product> SelectedProduct { get; set; }
        public async Task<IActionResult> OnGetAsync(string name)
        {
            Currenturl = HttpContext.Request.GetDisplayUrl();
            
            if (string.IsNullOrEmpty(name))
            {
                // If no search term, return empty results
                SelectedProduct = new List<Product>();
                return Page();
            }

            // Find products matching the search term
            SelectedProduct = await _context.TblProducts
                .Where(p => p.ProductName.Contains(name) || 
                           p.ShortDescription.Contains(name) || 
                           p.Tags.Contains(name))
                .ToListAsync();

            // Display the SearchResult page with search results
            return Page();
        }
        public async Task<IActionResult> OnPostAddToWishlistAsync(string SkuCode, string ProductName, bool buynow)
        {
            try
            {

                string UserEmail = HttpContext.Session.GetString("UserEmail");

                if (string.IsNullOrEmpty(UserEmail))
                {
                    // User is not logged in, redirect to login page
                    string redirectUrl = GetReturnUrlOrFallback("/Index"); // After login, redirect to the originating page
                    return RedirectToPage("/myaccount", new { redirectUrl });
                }
                // Use the AddToWishlist service method to add the product to the wishlist
                var result = await _addToWishlist.OnPostAddToWishlist(SkuCode, ProductName);

                // Check if the product was successfully added to the wishlist
                if (result is BadRequestResult)
                {
                    ModelState.AddModelError(string.Empty, "Failed to add product to wishlist.");
                    return LocalRedirect(GetReturnUrlOrFallback("/SearchResult"));
                }





                // If 'buynow' is false, redirect to the wishlist page
                return LocalRedirect(GetReturnUrlOrFallback("/SearchResult"));
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product to the wishlist.");
                return LocalRedirect(GetReturnUrlOrFallback("/SearchResult"));
            }
        }
        public async Task<IActionResult> OnPostAddToCart(int Qty, string MaterialName, string Size, string SkuCode, string Price, string AddOn, bool buynow = false)
        {
            try
            {
                var useremail = HttpContext.Session.GetString("UserEmail");
                var username = HttpContext.Session.GetString("UserName");
                var phone = HttpContext.Session.GetString("Phone");
                string Childsku = await _context.TblProductSizes.Where(e => e.ProductID == SkuCode && e.Size == Size).Select(e => e.SKUCode).FirstOrDefaultAsync();
                if (useremail == null)
                {
                    string redirectUrl = GetReturnUrlOrFallback("/Index"); // After login, redirect to the originating page
                    return RedirectToPage("/myaccount", new { redirectUrl });

                }

                await _addToCartItems.OnPostAddToCarts(Qty, MaterialName, Size, SkuCode, buynow, Price, Childsku, AddOn, useremail, phone, username);






                return LocalRedirect(GetReturnUrlOrFallback("/SearchResult"));


            }
            catch (Exception ex)
            {
                return Page();
            }
        }

        private string GetReturnUrlOrFallback(string fallbackPage)
        {
            var referer = Request.Headers.Referer.ToString();
            if (!string.IsNullOrWhiteSpace(referer) && Uri.TryCreate(referer, UriKind.Absolute, out var uri))
            {
                return uri.PathAndQuery;
            }

            return Url.Page(fallbackPage) ?? "/";
        }
    }
}

