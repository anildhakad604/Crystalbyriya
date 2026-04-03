using Astaberry.Helpers;
using CrystalByRiya.@class;
using CrystalByRiya.Classes;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Pages
{
    public class detailModel : PageModel
    {
        ApplicationDbContext _context;
        private readonly AddToWishlistModel _addToWishlist;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AddToCartItems _addToCartItems;

        private readonly ILogger<productlistModel> _logger;

        private readonly AmazonS3 _amazonS3;
        private readonly AwsCredentials awsCredentials;

     

        public detailModel(ApplicationDbContext context, AddToWishlistModel addToWishlist, ILogger<productlistModel> logger, IWebHostEnvironment webHostEnvironment, AddToCartItems addToCartItems, AwsCredentials awsCredentials, AmazonS3 amazonS3)
        {
            _context = context;
            _addToWishlist = addToWishlist;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _addToCartItems = addToCartItems;
            this.awsCredentials = awsCredentials;
            _amazonS3 = amazonS3;


        }
        private double ProductPrice = 0;
        private double Gstaddon;
        private double ProductGst = 0;
        private double TotalGst = 0;
        private double Netamount = 0;
        string ImgName = string.Empty;
        string ParentProductCode = string.Empty;
        string ProductName = string.Empty;
        string MaterialName = string.Empty;
        string Size = string.Empty;

        public int Categoryid { get; set; }
        public int SubCategoryid { get; set; }
        public string Currenturl1 { get; private set; }

        public string Currenturl { get; private set; }
        public Product Products { get; set; } = new Product();
        public List<ChildskuCode> ChildskuCodes { get; set; } = new List<ChildskuCode>();
        public List<Item> Usercart { get; set; } = new List<Item>();
        public Cart CartMarketing { get; set; }


        public IList<ProductFaq> ProductFaqs { get; set; } = new List<ProductFaq>();
        public IList<ProductSizes> ProductSizes { get; set; } = new List<ProductSizes>();
        public IList<ImageGallery> ImageGallery { get; set; } = new List<ImageGallery>();
        public IList<TblReviews> Reviews { get; set; } = new List<TblReviews>();
        public IList<ReviewGallery> ReviewGallery { get; set; } = new List<ReviewGallery>();
        public IList<Material> Materialname { get; set; } = new List<Material>();
        public string SelectedSize { get; set; }
        public string SelectedMaterial { get; set; }
        public string SelectedAddOn { get; set; }
        public IList<AddOn> AddOns { get; set; } = new List<AddOn>();
        public IList<Product> RecentlyViewedProducts { get; set; } = new List<Product>();
        public IList<CombinedReview> CombinedReviews { get; set; } = new List<CombinedReview>();

        public IList<Product> RelatedProducts { get; set; } = new List<Product>();

        public async Task OnGet(string skucode, string productname, string size = null, string material = null, string addon = null)
        {
            Currenturl = HttpContext.Request.GetDisplayUrl();
            productname = productname?.Replace('-', ' ') ?? string.Empty;
            Currenturl1 = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/detail/{skucode}/{productname}";
            Products = await _context.TblProducts.SingleOrDefaultAsync(e => e.SkuCode == skucode);

            if (Products == null)
            {
                return;
            }

            ProductSizes = await _context.TblProductSizes.Where(e => e.ProductID == skucode).ToListAsync();
            ImageGallery = await _context.TblImageGalleries.Where(e => e.Productid == skucode).OrderByDescending(e => e.Id).ToListAsync();
            ProductFaqs = await _context.TblProductFaq.Where(e => e.ProductSku == skucode).ToListAsync();
            Materialname = await _context.Materials.Where(e => e.SkuCode == skucode).ToListAsync();
            AddOns = await _context.TblAddOn.Where(e => e.ProductId == skucode).ToListAsync();

            
            if (!string.IsNullOrEmpty(size))
            {
                SelectedSize = size;
            }

            if (!string.IsNullOrEmpty(material))
            {
                SelectedMaterial = material;
            }
            if (!string.IsNullOrEmpty(addon))
            {
                SelectedAddOn = addon;
            }
            // Manage Recently Viewed
            string recentlyViewed = HttpContext.Session.GetString("RecentlyViewed");
            List<string> skuCodes = recentlyViewed != null ? recentlyViewed.Split(',').ToList() : new List<string>();

            if (!skuCodes.Contains(skucode))
            {
                skuCodes.Add(skucode);

                // Limit the recently viewed list to the last 5 products
                if (skuCodes.Count > 5)
                {
                    skuCodes = skuCodes.Skip(skuCodes.Count - 5).ToList();
                }

                HttpContext.Session.SetString("RecentlyViewed", string.Join(",", skuCodes));
            }
            // Fetch Reviews
            var reviews = await _context.TblReviews
                .Where(r => r.Productid == skucode)
                .ToListAsync();

            // Fetch and Group Gallery Images by ProductId
            var reviewGallery = await _context.TblReviewGallery
                .Where(g => g.ProductId == skucode)
                .GroupBy(g => g.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ImageName = g.FirstOrDefault().ImageName, // Take the first image (or customize logic)
                    IsApprovedGallery = g.FirstOrDefault().Approved
                }).ToListAsync();

            // Perform the Join Without Duplicates
            CombinedReviews = (from review in reviews
                               join gallery in reviewGallery
                               on review.Productid equals gallery.ProductId into galleryGroup
                               from gallery in galleryGroup.DefaultIfEmpty()
                               select new CombinedReview
                               {
                                   ProductId = review.Productid,
                                   ReviewText = review.Reviews,
                                   Rating = review.Rating,
                                   Email = review.Emailid,
                                   ReviewerName = review.Name,
                                   ReviewDate = review.ReviewDate,
                                   IsApprovedReview = review.IsApproved,
                                   ImageName = gallery?.ImageName, // Ensure only grouped image
                                   IsApprovedGallery = gallery?.IsApprovedGallery ?? false
                               }).ToList();


            // Fetch product details for recently viewed products
            RecentlyViewedProducts = await _context.TblProducts
                .Where(p => skuCodes.Contains(p.SkuCode))
                .ToListAsync();

            // Fetch related SKU codes from the mapping table first.
            // If no explicit mappings exist for this product, fall back to products in the same family/tag.
            var relatedSkuCodes = await _context.Set<RelatedProduct>()
                .Where(r => Products != null && r.BlogId == Products.ID)
                .Select(r => r.Skucode)
                .ToListAsync();

            if (relatedSkuCodes.Any())
            {
                RelatedProducts = await _context.TblProducts
                    .Where(p => relatedSkuCodes.Contains(p.SkuCode) && p.SkuCode != skucode)
                    .ToListAsync();
            }
            else
            {
                var fallbackQuery = _context.TblProducts
                    .Where(p => p.SkuCode != skucode);

                if (!string.IsNullOrWhiteSpace(Products.ParentCode))
                {
                    RelatedProducts = await fallbackQuery
                        .Where(p => p.ParentCode == Products.ParentCode)
                        .Take(8)
                        .ToListAsync();
                }

                if (RelatedProducts == null || !RelatedProducts.Any())
                {
                    RelatedProducts = await fallbackQuery
                        .Where(p => !string.IsNullOrWhiteSpace(Products.Tags) && p.Tags == Products.Tags)
                        .Take(8)
                        .ToListAsync();
                }
            }


        }
        public async Task<IActionResult> OnPostAddToCart(int Qty, string MaterialName, string Size, string SkuCode, bool buynow, string Price, string Childsku, string AddOn)
        {
            try
            {
                var useremail = HttpContext.Session.GetString("UserEmail");
                var username = HttpContext.Session.GetString("UserName");
                var phone = HttpContext.Session.GetString("Phone");
                if (useremail == null && buynow==false)
                {
                    string redirectUrl = Url.Page("/cart") ?? "/cart"; // After login, redirect straight to cart
                   
                    return RedirectToPage("/myaccount", new { redirectUrl });

                }
               
                    await _addToCartItems.OnPostAddToCarts(Qty, MaterialName, Size, SkuCode,buynow, Price, Childsku, AddOn, useremail, phone, username);
                
                        // Handle "Buy Now"
                        if (buynow==true)
                    {
                       
                        return RedirectToPage("/checkout", new { isBuyNow = true });
                    }

                    // Handle regular cart logic
                   

                   
                   
                    return LocalRedirect(GetReturnUrlOrFallback("/Index"));
                

            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAddToWishlistAsync(string SkuCode, string ProductName, bool buynow)
        {
            try
            {

                string UserEmail = HttpContext.Session.GetString("UserEmail");

                if (string.IsNullOrEmpty(UserEmail) )
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
                    return LocalRedirect(GetReturnUrlOrFallback("/Index"));
                }





                // If 'buynow' is false, redirect to the wishlist page
                return LocalRedirect(GetReturnUrlOrFallback("/Index"));
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product to the wishlist.");
                return LocalRedirect(GetReturnUrlOrFallback("/Index"));
            }
        }
        public async Task<IActionResult> OnPostReview(string ProductId, int Rating, string ReviewText, IList<IFormFile> Images)
        {
            try
            {
                // Ensure ProductId, Rating, and Email are passed through the form
                if (string.IsNullOrEmpty(ProductId))
                {
                    ModelState.AddModelError(string.Empty, "ProductId, Email, and Rating are required.");
                    return Page(); // Return the page with an error if required fields are missing
                }
                string UserEmail = HttpContext.Session.GetString("UserEmail");
                string UserName = HttpContext.Session.GetString("UserName");
                if (Images != null && Images.Count > 0)
                {
                    // Logic to handle image uploads
                    foreach (var image in Images)
                    {
                        string fileName = await _amazonS3.UploadFileToS3(image, awsCredentials.ReviewFoldername);
                        

                     
                       


                        // Add the image to the ReviewGallery table
                        var reviewGallery = new ReviewGallery
                        {
                            ImageName = fileName,
                            Email = UserEmail,
                            ProductId = ProductId,
                            Approved = false
                        };

                        _context.TblReviewGallery.Add(reviewGallery);

                    }
                    var review = new TblReviews
                    {
                        Productid = ProductId,
                        Reviews = ReviewText,
                        IsApproved = false,
                        Rating = Rating,
                        Emailid = UserEmail,
                        Name = UserName,
                        ReviewDate = DateTime.Now
                    };
                    _context.TblReviews.Add(review);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // If no images were uploaded, save the review to the TblReviews table
                    var newReview = new TblReviews
                    {
                        Productid = ProductId,
                        Reviews = ReviewText,
                        IsApproved = false,
                        Rating = Rating,
                        Emailid = UserEmail,
                        Name = UserName,
                        ReviewDate = DateTime.Now
                    };

                    _context.TblReviews.Add(newReview);
                    await _context.SaveChangesAsync();
                }

                return RedirectToPage(new { success = true });
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
