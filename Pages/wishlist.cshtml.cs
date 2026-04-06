using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Astaberry.Helpers;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Pages
{
    public class wishlistModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public wishlistModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<AddToWishlist> WishlistItems { get; private set; } = new List<AddToWishlist>();
        public List<AddingWishlist> Tblwishlist { get; set; } = new List<AddingWishlist>();
        public string Currenturl { get; private set; }
        public string checkwishlist { get; set; }
        public string username { get; set; }
        public string useremail { get; set; }
        public string phone { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get the current URL
            Currenturl = HttpContext.Request.GetDisplayUrl();

            // Load wishlist items from session first so guest wishlists stay intact.
            var sessionWishlist = SessionHelper.GetObjectFromJson<List<AddToWishlist>>(HttpContext.Session, "wishlist") ?? new List<AddToWishlist>();
            WishlistItems = sessionWishlist;
            checkwishlist = HttpContext.Session.GetString("WishlistStatus");
            useremail = HttpContext.Session.GetString("UserEmail");

            if (!string.IsNullOrWhiteSpace(useremail))
            {
                Tblwishlist = await _context.TblWishlist
                    .Where(e => e.UserEmail == useremail)
                    .Select(e => new AddingWishlist
                    {
                        skucode = e.skucode,
                        ProductName = e.ProductName,
                        Price = e.Price,
                        Image = e.Image
                    })
                    .ToListAsync();
                if (Tblwishlist != null && Tblwishlist.Any())
                {
                    WishlistItems = Tblwishlist.Select(item => new AddToWishlist
                    {
                        ProductId = item.skucode ?? string.Empty,
                        ProductName = item.ProductName ?? string.Empty,
                        Price = item.Price ?? string.Empty,
                        Image = item.Image ?? string.Empty
                    }).ToList();
                }
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, "wishlist", WishlistItems);

            return Page();
        }
        public async Task<IActionResult> OnGetDeleteFromWishlist(string id)
        {
            try
            {
                // Retrieve the wishlist from the session
                WishlistItems = SessionHelper.GetObjectFromJson<List<AddToWishlist>>(HttpContext.Session, "wishlist") ?? new List<AddToWishlist>();

                // Retrieve the user's email from the session
                useremail = HttpContext.Session.GetString("UserEmail");

                // Find the item in the database by user email and SKU code
                var dbWishlistItem = await _context.TblWishlist.FirstOrDefaultAsync(e =>
                    e.UserEmail == useremail && e.skucode == id);

                if (dbWishlistItem != null)
                {
                    // Remove the item from the database
                    _context.TblWishlist.Remove(dbWishlistItem);
                    await _context.SaveChangesAsync();
                }

                // Find the item in the wishlist from the session by its ID
                int index = Existed.Exists(WishlistItems, id);

                if (index >= 0)
                {
                    // Remove the item from the session wishlist
                    WishlistItems.RemoveAt(index);

                    // Save the updated wishlist back to the session
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "wishlist", WishlistItems);
                }

                // Redirect back to the Wishlist page
                return RedirectToPage("Wishlist");
            }
            catch (Exception ex)
            {
                // Log the exception if logging is implemented
                return BadRequest("An error occurred while trying to delete the item from the wishlist.");
            }
        }

    }
}
