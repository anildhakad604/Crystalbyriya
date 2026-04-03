using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CrystalByRiya.Models;
using System.Linq;
using Astaberry.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Pages
{
    public class CartModel : PageModel
    {
        ApplicationDbContext _context;

        public CartModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string Currenturl { get; private set; }
        public CouponCodes Coupon { get; set; }
        public string CouponMessage { get; set; }
        public List<Item> Carts { get; set; } = new List<Item>();
        public List<CouponCodes> AvailableCoupons { get; set; } = new List<CouponCodes>();
        public List<ChildskuCode> ChildSkuCodes { get; set; } = new List<ChildskuCode>();
        public List<Cart> CartMarketing { get; set; } = new List<Cart>();
        public double GstAmount { get; set; }
        public double Subtotal { get; set; }
        public double Shipping { get; set; }
        public double Total { get; set; }
        public double CouponDiscount { get; set; }
        public double DiscountAmount { get; set; }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                var productID = HttpContext.Session.GetString("productid");
                var quantity = HttpContext.Session.GetInt32("quantity");
                var childsku = HttpContext.Session.GetString("childsku");
                // Get the current URL
                Currenturl = HttpContext.Request.GetDisplayUrl();
                var useremail = HttpContext.Session.GetString("UserEmail");
                Carts = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") ?? new List<Item>();

                if (useremail != null && Carts.Count == 0)
                {
                    CartMarketing = await _context.TblCarts.Where(e => e.UserEmail == useremail).ToListAsync();
                    if (CartMarketing.Count > 0 && CartMarketing.Any())
                    {
                        Carts = CartMarketing.Select(cart => new Item
                        {
                            ProductName = cart.ProductName,
                            ProductId = cart.ProductId,
                            Qty = cart.Qty,
                            skucode = cart.ProductId,
                            Price = cart.Price,
                            Gst = cart.Gst,
                            MaterialName = cart.material,
                            Size = cart.size,
                            Image = cart.Image
                        }).ToList();

                        SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", Carts);
                    }
                    else
                    {
                        Carts = new List<Item>();
                    }

                }

                // If user is logged in and session cart has items, sync with database
                else if (useremail != null && Carts.Count > 0)
                {
                    // User is logged in and has items in session cart - sync with database
                    CartMarketing = await _context.TblCarts.Where(e => e.UserEmail == useremail).ToListAsync();
                    if (CartMarketing.Count > 0 && CartMarketing.Any())
                    {
                        Carts = CartMarketing.Select(cart => new Item
                        {
                            ProductName = cart.ProductName,
                            ProductId = cart.ProductId,
                            Qty = cart.Qty,
                            skucode = cart.ProductId,
                            Price = cart.Price,
                            Gst = cart.Gst,
                            MaterialName = cart.material,
                            Size = cart.size,
                            Image = cart.Image
                        }).ToList();

                        SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", Carts);
                    }
                }
                // If user is not logged in, keep session cart as is (or empty)
                // No database query needed for logged-out users
                if (!string.IsNullOrEmpty(productID) && quantity.HasValue)
                {
                    // Find the item in the cart based on ProductID
                    var cartItem = Carts.FirstOrDefault(i => i.ProductId == productID);

                    if (cartItem != null)
                    {
                        // Update the quantity and SKU code
                        cartItem.Qty = quantity.Value;
                    }
                }

                var now = DateTime.Now;
                AvailableCoupons = await _context.CouponCodes
                    .AsNoTracking()
                    .Where(c => c.IsActive && c.ApplicableFrom <= now && c.ApplicableTo >= now)
                    .OrderByDescending(c => c.DiscountPercentage)
                    .ThenBy(c => c.ApplicableTo)
                    .ToListAsync();

                // Calculate totals
                CalculateTotals();

                // Update the cart in the session
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", Carts);
                return Page();
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return Page();
            }
        }

        private void CalculateTotals()
        {
            string check = HttpContext.Session.GetString("AppliedCoupon");
            var discountvalue = HttpContext.Session.GetInt32("Discount");
            var merchandiseSubtotal = 0.0;
            Total = 0;
            GstAmount = 0;

            foreach (var item in Carts)
            {
                if (item == null) continue;

                merchandiseSubtotal += item.Price * item.Qty;
                GstAmount += Convert.ToDouble(item.Gst);
            }

            Subtotal = merchandiseSubtotal;

            if (check == "True" && discountvalue.HasValue)
            {
                DiscountAmount = Subtotal * discountvalue.Value / 100.0;
            }
            else
            {
                DiscountAmount = 0;
            }

            var discountedSubtotal = Subtotal - DiscountAmount;
            Shipping = discountedSubtotal < 3000 ? 80 : 0;
            Total = discountedSubtotal + Shipping;

            if (check == "True")
            {
                HttpContext.Session.SetString("AppliedCoupon", "True");
            }
        }
        public async Task<IActionResult> OnPostCouponCode(string coupon_code)
        {
            var normalizedCouponCode = coupon_code?.Trim();
            var carts = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") ?? new List<Item>();
            var useremail = HttpContext.Session.GetString("UserEmail");

            if ((carts == null || !carts.Any()) && !string.IsNullOrWhiteSpace(useremail))
            {
                CartMarketing = await _context.TblCarts.Where(e => e.UserEmail == useremail).ToListAsync();
                if (CartMarketing.Any())
                {
                    carts = CartMarketing.Select(cart => new Item
                    {
                        ProductName = cart.ProductName,
                        ProductId = cart.ProductId,
                        Qty = cart.Qty,
                        skucode = cart.ProductId,
                        Price = cart.Price,
                        Gst = cart.Gst,
                        MaterialName = cart.material,
                        Size = cart.size,
                        Image = cart.Image
                    }).ToList();

                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", carts);
                }
            }

            if (carts == null || !carts.Any())
            {
                HttpContext.Session.Remove("AppliedCoupon");
                HttpContext.Session.Remove("Discount");
                TempData["CouponMessage"] = "Your cart is empty. Add items to the cart before applying a coupon.";
                return RedirectToPage("cart");
            }

            if (string.IsNullOrWhiteSpace(normalizedCouponCode))
            {
                HttpContext.Session.Remove("AppliedCoupon");
                HttpContext.Session.Remove("Discount");
                TempData["CouponMessage"] = "Please enter a coupon code.";
                return RedirectToPage("cart");
            }

            Coupon = await _context.CouponCodes
                .FirstOrDefaultAsync(e => e.CouponCode == normalizedCouponCode);

            if (Coupon == null)
            {
                HttpContext.Session.Remove("AppliedCoupon");
                HttpContext.Session.Remove("Discount");
                TempData["CouponMessage"] = "Invalid coupon code.";
            }
            else if (!Coupon.IsActive)
            {
                HttpContext.Session.Remove("AppliedCoupon");
                HttpContext.Session.Remove("Discount");
                TempData["CouponMessage"] = "This coupon is no longer active.";
            }
            else if (DateTime.Now < Coupon.ApplicableFrom || DateTime.Now > Coupon.ApplicableTo)
            {
                HttpContext.Session.Remove("AppliedCoupon");
                HttpContext.Session.Remove("Discount");
                TempData["CouponMessage"] = "This coupon is not valid for the selected date.";
            }
            else
            {
                // Set coupon details in session
                HttpContext.Session.SetString("AppliedCoupon", "True");
                HttpContext.Session.SetInt32("Discount", Coupon.DiscountPercentage);

                // Recalculate totals
                CalculateTotals();

                TempData["CouponMessage"] = "Coupon applied successfully! Discount has been applied to your total.";
            }

            return RedirectToPage("cart");
        }


        public async Task<IActionResult> OnPostUpdateQuantity(string productID, int quantity, string size)
        {
            try
            {
                // Get the cart from the session
                var Usercart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

                if (Usercart == null || !Usercart.Any())
                {
                    return BadRequest("Cart is empty.");
                }

                // Find the item in the session cart using ProductID and size
                var cartItem = Usercart.FirstOrDefault(i => i.ProductId == productID && i.Size == size);

                if (cartItem != null)
                {
                    // Update the quantity in the session cart
                    cartItem.Qty = quantity;

                    // Update the cart in the session
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", Usercart);

                    // Get the user's email from the session
                    var userEmail = HttpContext.Session.GetString("UserEmail");

                    // Find the corresponding item in the TblCarts table
                    var dbCartItem = await _context.TblCarts.FirstOrDefaultAsync(c =>
                        c.UserEmail == userEmail &&
                        c.ProductId == productID &&
                        c.size == size);

                    if (dbCartItem != null)
                    {
                        // Update the quantity in the database
                        dbCartItem.Qty = quantity;

                        // Save changes to the database
                        await _context.SaveChangesAsync();
                    }

                    // Redirect to the Cart page to prevent form re-submission issues
                    return RedirectToPage("Cart");
                }
                else
                {
                    return BadRequest("Item not found in the cart.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return Page();
            }
        }


        public async Task<IActionResult> OnGetDelete(string id, string size, string addon, string material)
        {
            try
            {
                // Retrieve the cart from the session
                Carts = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

                if (Carts == null || !Carts.Any())
                {
                    return BadRequest("Cart is empty or not found.");
                }

                // Retrieve the user email from session
                var useremail = HttpContext.Session.GetString("UserEmail");

                // Remove the item from the database if it exists
                var itemInDb = await _context.TblCarts.FirstOrDefaultAsync(e =>
                    e.UserEmail == useremail &&
                    e.addon == addon &&
                    e.size == size &&
                    e.material == material &&
                    e.ProductId == id);

                if (itemInDb != null)
                {
                    _context.TblCarts.Remove(itemInDb);
                    await _context.SaveChangesAsync();
                }

                // Find the item in the session cart
                var itemInSession = Carts.FirstOrDefault(c =>
                    c.ProductId == id &&
                    c.Size == size &&
                    c.MaterialName == material &&
                    c.Addon == addon);

                if (itemInSession != null)
                {
                    Carts.Remove(itemInSession);
                    // Update the session cart
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", Carts);
                }

                // Redirect back to the Cart page
                return RedirectToPage("Cart");
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return Page();
            }
        }
    }
    public class ChildskuCode
    {
        public string SKUCode { get; set; }
        public string ProductID { get; set; }
        public string Size { get; set; }
        public string Material { get; set; }
        public string Product { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public float Gst { get; set; }
        public string Addon { get; set; }
    }
}
