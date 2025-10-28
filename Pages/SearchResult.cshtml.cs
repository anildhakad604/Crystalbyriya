using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CrystalByRiya.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.@class;

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
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("SkuCode is missing.");
            }

            // Find the selected product by SkuCode
            SelectedProduct = await _context.TblProducts
            .Where(p => p.ProductName.Contains(name)) // Partial match on the product name
            .ToListAsync();

            if (SelectedProduct == null)
            {
                return NotFound();  // If product is not found, return 404
            }

            // Display the SearchResult page with product details
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
                    string redirectUrl = Url.Page("/Index"); // After login, redirect to the wishlist
                    return RedirectToPage("/myaccount", new { redirectUrl });
                }
                // Use the AddToWishlist service method to add the product to the wishlist
                var result = await _addToWishlist.OnPostAddToWishlist(SkuCode, ProductName);

                // Check if the product was successfully added to the wishlist
                if (result is BadRequestResult)
                {
                    ModelState.AddModelError(string.Empty, "Failed to add product to wishlist.");
                    return RedirectToPage("/blogdetail");
                }





                // If 'buynow' is false, redirect to the wishlist page
                return RedirectToPage("/SearchResult");
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product to the wishlist.");
                return RedirectToPage("/SearchResult");
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
                    string redirectUrl = Url.Page("Index"); // After login, redirect to the wishlist
                    return RedirectToPage("/myaccount", new { redirectUrl });

                }

                await _addToCartItems.OnPostAddToCarts(Qty, MaterialName, Size, SkuCode, buynow, Price, Childsku, AddOn, useremail, phone, username);






                return RedirectToPage("SearchResult");


            }
            catch (Exception ex)
            {
                return Page();
            }
        }
    }
}

