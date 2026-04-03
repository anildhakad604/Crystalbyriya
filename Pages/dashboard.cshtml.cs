using Astaberry.Helpers;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;


namespace CrystalByRiya.Pages
{
    public class dashboardModel : PageModel
    {

        private readonly ApplicationDbContext _context;

        public dashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }


        [BindProperty]
        public TblBillingDetail Detail { get; set; }

        public Register Register { get; set; }

        public string UserEmail { get; private set; }
        public string Password { get; private set; }

        // GET: Load the page and prepopulate the email if available
        public IActionResult OnGet()
        {
            try
            {
                // Retrieve the session value for "UserEmail"
                UserEmail = HttpContext.Session.GetString("UserEmail");
                Password = HttpContext.Session.GetString("Password");

                // Check if user is logged in
                if (string.IsNullOrEmpty(UserEmail))
                {
                    // User is not logged in, redirect to login page
                    return RedirectToPage("/myaccount");
                }

                // Fetch the most recent billing detail for the user based on Emailid
                if (!string.IsNullOrEmpty(UserEmail))
                {
                    Detail = _context.TblBillingDetails
                                        .Where(b => b.Emailid == UserEmail)
                                   .FirstOrDefault();
                    Register = _context.TblRegisters.FirstOrDefault(c => c.Email == UserEmail);
                    // If no detail is found, initialize a new one with the email
                    if (Detail == null)
                    {
                        Detail = new TblBillingDetail
                        {
                            Emailid = UserEmail
                        };
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Log error if needed
                return Page();
            }
        }
        // Handle the form submission to update account details
        public List<AddToWishlist> WishlistItems { get; private set; }
        public string Currenturl { get; private set; }

        public IActionResult OnGetWishlist()
        {
            try
            {
                // Get the current URL
                Currenturl = HttpContext.Request.GetDisplayUrl();

                // Load wishlist items from session
                WishlistItems = SessionHelper.GetObjectFromJson<List<AddToWishlist>>(HttpContext.Session, "wishlist");

                // If the wishlist is empty, create an empty list
                if (WishlistItems == null)
                {
                    WishlistItems = new List<AddToWishlist>();
                }

                // Serialize the wishlist items to JSON format
                var jsonResult = JsonSerializer.Serialize(WishlistItems);

                // Return the JSON result as a ContentResult
                return new ContentResult
                {
                    Content = jsonResult,
                    ContentType = "application/json",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);
                return new ContentResult
                {
                    Content = "An error occurred while loading the wishlist.",
                    ContentType = "text/plain",
                    StatusCode = 500
                };
            }
        }
        public IActionResult OnPostRemoveFromWishlist(string productId)
        {
            try
            {
                // Load wishlist items from session
                WishlistItems = SessionHelper.GetObjectFromJson<List<AddToWishlist>>(HttpContext.Session, "wishlist");

                // If the wishlist is not empty, find and remove the item
                if (WishlistItems != null)
                {
                    int index = WishlistItems.FindIndex(item => item.ProductId == productId);

                    // Remove the item if it exists
                    if (index >= 0)
                    {
                        WishlistItems.RemoveAt(index);
                    }

                    // Save the updated wishlist back to the session
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "wishlist", WishlistItems);
                }

                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (!string.IsNullOrWhiteSpace(userEmail) && !string.IsNullOrWhiteSpace(productId))
                {
                    var dbWishlistItem = _context.TblWishlist.FirstOrDefault(w =>
                        w.UserEmail == userEmail && w.skucode == productId);

                    if (dbWishlistItem != null)
                    {
                        _context.TblWishlist.Remove(dbWishlistItem);
                        _context.SaveChanges();
                    }
                }

                // Redirect back to the wishlist page
                return RedirectToPage("Wishlist"); // Change to the appropriate page name
            }
            catch (Exception ex)
            {
                // Log the error if necessary
                Console.WriteLine("Error removing item from wishlist: " + ex.Message);
                return RedirectToPage(); // Redirect to an error page or handle the error as needed
            }
        }
        public IActionResult OnPostEdit()
        {

            var userEmail = HttpContext.Session.GetString("UserEmail");

            // Fetch the existing billing details for the user
            var billingDetails = _context.TblBillingDetails.FirstOrDefault(b => b.Emailid == userEmail);

            if (billingDetails != null)
            {
                // Update the billing details with the form data
                billingDetails.FullName = Detail.FullName;
                billingDetails.ContactNumber = Detail.ContactNumber;
                billingDetails.Address = Detail.Address;
                billingDetails.City = Detail.City;
                billingDetails.State = Detail.State;
                billingDetails.PinCode = Detail.PinCode;

                // Save the updated details to the database
                _context.SaveChanges();
            }
            else
            {

                // Create new billing details if none exist
                billingDetails = new TblBillingDetail
                {
                    Emailid = userEmail,
                    FullName = Detail.FullName,
                    ContactNumber = Detail.ContactNumber,
                    Address = Detail.Address,
                    City = Detail.City,
                    State = Detail.State,
                    PinCode = Detail.PinCode,
                    Country = "India",
                    Name = "dummy",
                    LastName = "dummy"


                };

                // Add new billing details to the database
                _context.TblBillingDetails.Add(billingDetails);
            }

            try
            {
                // Save the updated details to the database
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Log the detailed inner exception message
                Console.WriteLine(ex.InnerException?.Message);
                // Return an error message or handle it accordingly
                return RedirectToPage(new { error = "An error occurred while saving billing details. Please try again." });
            }

            // Redirect to the same page to refresh the form with updated details
            return RedirectToPage();
        }

        // Return the page with validation errors, if any
        public IActionResult OnPostLogout()
        {
            try
            {
                // Clear all session data including cart and wishlist
                HttpContext.Session.Clear();

                // Redirect to index page (or login page) after logout
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                return Page();
            }
        }
    }
}

