using Astaberry.Helpers;
using CrystalByRiya.@class;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;

namespace CrystalByRiya.Pages
{
    public class blogdetailModel : PageModel
    {
        ApplicationDbContext _context;
        private readonly AddToWishlistModel _addToWishlist;
        private readonly AddToCartItems _addToCartItems;
        public blogdetailModel(ApplicationDbContext context, AddToWishlistModel addToWishlist, AddToCartItems addToCartItems)
        {
            _context = context;
            _addToWishlist = addToWishlist;
            _addToCartItems = addToCartItems;
        }
        string ImgName = string.Empty;
        string ParentProductCode = string.Empty;
        string ProductName = string.Empty;
        private double ProductPrice = 0;
        private double Gstaddon;
        private double ProductGst = 0;
        private double TotalGst = 0;
        private double Netamount = 0;
        string MaterialName = string.Empty;
        string Size = string.Empty;
        public Cart CartMarketing { get; set; }
        public List<Item> Usercart { get; set; } = new List<Item>();
        public string Currenturl { get; private set; }
        [BindProperty]
        public Blogs Singleblog { get; set; }
        public List<CommentReply> ReplyList { get; set; }
        public List<BlogFaq> BlogFaqs { get; set; }
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Comment { get; set; }
        [BindProperty]
        public List<Product>    Products { get; set; }
        public List<string> TblRelatedProducts { get; set; }

        public async Task<IActionResult> OnGet(int blogid)
        {
            try
            {
                Currenturl = HttpContext.Request.GetDisplayUrl();
                Singleblog = await _context.TblBlogs.FindAsync(blogid);
                BlogFaqs = await _context.TblBlogsFaq.Where(e => e.BlogId == blogid).ToListAsync();
                ReplyList = await _context.TblCommentReply.Where(e => e.BlogId == blogid).ToListAsync();
                TblRelatedProducts= await _context.RelatedProducts.Where(e => e.BlogId == blogid).Select(e=>e.Skucode).ToListAsync();
                if(TblRelatedProducts.Count > 0)
                {
                    Products = await _context.TblProducts
                .Where(p => TblRelatedProducts.Contains(p.SkuCode))
                .ToListAsync();
                }
                else
                {
                    Products = new List<Product>();
                }
            }
            catch (Exception ex) { }
            return Page();
        }
        public async Task<IActionResult> OnPostAddComment(int blogid,string title)
        {
            Currenturl = HttpContext.Request.GetDisplayUrl();
            var newComment = new CommentReply
                {
                    BlogId = blogid,
                    Name = Name,        // Matches "Name" in CommentReply
                    Email = Email,      // Matches "Email" in CommentReply
                    Comment = Comment,  // Matches "Comment" in CommentReply
                    Date = DateTime.Now,
                IsApproved=false
            };

                _context.TblCommentReply.Add(newComment);
                await _context.SaveChangesAsync();

            return RedirectToPage(null, new { blogid, url=title});



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
                return RedirectToPage("/blogdetail");
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product to the wishlist.");
                return RedirectToPage("/blogdetail");
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






                return RedirectToPage("blogdetail");


            }
            catch (Exception ex)
            {
                return Page();
            }
        }
    }

    }

