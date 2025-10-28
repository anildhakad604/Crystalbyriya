using Astaberry.Helpers;
using CrystalByRiya.@class;
using CrystalByRiya.Models;
using CrystalByRiya.StoredProcedure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;

namespace CrystalByRiya.Pages
{
    public class productlistModel : PageModel
    {
        private readonly AddToWishlistModel _addToWishlist;
        ApplicationDbContext _context;
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger<productlistModel> _logger;
        private readonly AddToCartItems _addToCartItems;
        public List<Item> Userwishlist { get; set; }

        public productlistModel(ApplicationDbContext context, IAntiforgery antiforgery, ILogger<productlistModel> logger, AddToWishlistModel addToWishlist, AddToCartItems addToCartItems)
        {
            _context = context;
            _antiforgery = antiforgery;
            _logger = logger;
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
        public List<SPSubCategoryWiseProduct> SPSubCategoryWiseProduct { get; set; }
        public Cart CartMarketing { get; set; }
        public List<Item> Usercart { get; set; } = new List<Item>();
        public Product product { get; set; }
        public string Currenturl { get; private set; }
        public Subcategory Subcategory { get; set; }
        public TblCategory Category { get; set; }
        public List<Subcategory> SubCategoryList { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12; // Adjust this as needed
        public List<SearchFilter> PagedProducts { get; set; }
        public List<BestSeller> SpBestseller { get; set; }
        public List<TblReviews> Reviews { get; set; }
        public List <Product> products { get; set; }
        public List<QuickViews> QuickViews { get; set; }= new List<QuickViews>();
        public string Subcategorytitle { get; set; }

        public async Task<IActionResult> OnGet(string catname, string subcatname, int currentPage = 1)
        {
            try
            {
                // Get current URL
                Currenturl = HttpContext.Request.GetDisplayUrl();

                // Replace hyphens with spaces for category name
                catname = catname.Replace('-', ' ');

                // Fetch category
                Category = await _context.TblCategory.SingleOrDefaultAsync(e => e.MenuList == catname);

                if (Category == null)
                {
                    ModelState.AddModelError(string.Empty, "Category not found");
                    return Page();
                }

                // Clean up subcategory name by removing unwanted parts
                if (subcatname.Contains('-'))
                {
                    subcatname = subcatname.Replace("-", " ").Replace("Crystals", "");
                    Subcategorytitle = subcatname;

                }

                // Fetch subcategory
                Subcategory = await _context.TblSubcategory.SingleOrDefaultAsync(e => e.SubCategoryname == subcatname.TrimEnd().TrimStart());

                if (Subcategory == null)
                {
                    ModelState.AddModelError(string.Empty, "Subcategory not found");
                    return Page();
                }

                // Fetch subcategory list (though not clear why based on subcategory id, keeping it as in original code)
                SubCategoryList = await _context.TblSubcategory.Where(e => e.SubCategoryid == Subcategory.SubCategoryid).ToListAsync();
       var priceasc=         HttpContext.Session.GetString("priceasc");
                var pricedesc = HttpContext.Session.GetString("pricedesc");
                if (priceasc != "True" && pricedesc != "True")
                {
                    // Prepare SQL parameters
                    var parameters = new[]
                    {
            new SqlParameter("@CategoryId", Category.Id),
            new SqlParameter("@SubCategory", Subcategory.SubCategoryid)
        };


                    // Fetch products based on stored procedure
                    SPSubCategoryWiseProduct = await _context.SPSubCategoryWiseProduct
                        .FromSqlRaw("SPSubCategoryWiseProduct @CategoryId, @SubCategory", parameters)
                       .ToListAsync();
                    PagedProducts = SPSubCategoryWiseProduct.Select(p => new SearchFilter
                    {
                        Skucode = p.SkuCode,
                        Price = p.Price,
                        Thumbnail = p.Thumbnail,
                        ProductName = p.ProductName,

                    }).ToList();
                    CurrentPage = currentPage;
                    int skipCount = (CurrentPage - 1) * PageSize;
                    int totalProducts = SPSubCategoryWiseProduct.Count;
                    TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);
                    PagedProducts = PagedProducts.Skip(skipCount).Take(PageSize).ToList();
                }
                if (priceasc == "True" && pricedesc != "True")
                {
                    var parameters = new[]
                    {
            new SqlParameter("@CategoryId", Category.Id),
            new SqlParameter("@SubCategory", Subcategory.SubCategoryid)
        };
                    SPSubCategoryWiseProduct = await _context.SPSubCategoryWiseProduct
    .FromSqlRaw("SPSubCategoryWiseProduct @CategoryId, @SubCategory", parameters)
    .ToListAsync();

                    // Sort products by the lowest price in ascending order, handling single values and ranges
                    PagedProducts = SPSubCategoryWiseProduct
                        .OrderBy(p =>
                        {
                            // Check if the price is a range or single value
                            var priceParts = p.Price.Split('-');
                            if (decimal.TryParse(priceParts[0], out var lowestPrice)) // Extract the first part
                            {
                                return lowestPrice; // Use the lowest price if valid
                            }
                            return decimal.MaxValue; // Handle invalid formats by placing them last
                        })
                        .Select(p => new SearchFilter
                        {
                            Skucode = p.SkuCode,
                            Price = p.Price,
                            Thumbnail = p.Thumbnail,
                            ProductName = p.ProductName,
                        }).ToList();

                    // Pagination
                    CurrentPage = currentPage;
                    int skipCount = (CurrentPage - 1) * PageSize;
                    int totalProducts = PagedProducts.Count;
                    TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

                    // Apply pagination after sorting
                    PagedProducts = PagedProducts.Skip(skipCount).Take(PageSize).ToList();
                    HttpContext.Session.Remove("priceasc");

                }
                if (priceasc != "True" && pricedesc == "True")
                {
                    var parameters = new[]
                    {
            new SqlParameter("@CategoryId", Category.Id),
            new SqlParameter("@SubCategory", Subcategory.SubCategoryid)
        };
                    SPSubCategoryWiseProduct = await _context.SPSubCategoryWiseProduct
    .FromSqlRaw("SPSubCategoryWiseProduct @CategoryId, @SubCategory", parameters)
    .ToListAsync();

                    // Sort products by the lowest price in ascending order, handling single values and ranges
                    PagedProducts = SPSubCategoryWiseProduct
                        .OrderByDescending(p =>
                        {
                            // Check if the price is a range or single value
                            var priceParts = p.Price.Split('-');
                            if (decimal.TryParse(priceParts[0], out var lowestPrice)) // Extract the first part
                            {
                                return lowestPrice; // Use the lowest price if valid
                            }
                            return decimal.MaxValue; // Handle invalid formats by placing them last
                        })
                        .Select(p => new SearchFilter
                        {
                            Skucode = p.SkuCode,
                            Price = p.Price,
                            Thumbnail = p.Thumbnail,
                            ProductName = p.ProductName,
                        }).ToList();

                    // Pagination
                    CurrentPage = currentPage;
                    int skipCount = (CurrentPage - 1) * PageSize;
                    int totalProducts = PagedProducts.Count;
                    TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

                    // Apply pagination after sorting
                    PagedProducts = PagedProducts.Skip(skipCount).Take(PageSize).ToList();
                    HttpContext.Session.Remove("pricedesc");

                }
                // Handle the case where no products are found
                if (SPSubCategoryWiseProduct == null || !SPSubCategoryWiseProduct.Any())
                    {
                        SPSubCategoryWiseProduct = new List<SPSubCategoryWiseProduct>();
                        ModelState.AddModelError(string.Empty, "No products found for the selected category and subcategory.");
                    }
                
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while fetching the product list.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
            }

            return Page();
        }


        [ValidateAntiForgeryToken]

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
                    return RedirectToPage("/productlist");
                }





                // If 'buynow' is false, redirect to the wishlist page
                return RedirectToPage("/productlist");
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product to the wishlist.");
                return RedirectToPage("/productlist");
            }
        }

        public async Task<IActionResult> OnPostFilter(bool popularity, bool rating, bool ascedingprice, bool descendingprice, bool bydefault, bool latest, int currentPage = 1)
        {
            if (popularity)
            {
                var result = await _context.SpBestSeller.FromSqlRaw("SpBestseller").ToListAsync();
                PagedProducts = result.Select(p => new SearchFilter
                {
                    Skucode = p.Skucode,
                    Price = p.Price,
                    Thumbnail = p.Thumbnail,
                    ProductName = p.ProductName,

                }).ToList();
                CurrentPage = currentPage;
                int skipCount = (CurrentPage - 1) * PageSize;
                int totalProducts = result.Count;
                TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);
                PagedProducts = PagedProducts.Skip(skipCount).Take(PageSize).ToList();



                return Page();
            }
            if (rating)
            {
                var result = await (
     from review in _context.TblReviews
     join product in _context.TblProducts on review.Productid equals product.SkuCode
     where review.Rating >= 3
     select new { Review = review, Product = product } // Include both full entities
 ).ToListAsync();

                PagedProducts = result.Select(p => new SearchFilter
                {
                    Skucode = p.Product.SkuCode,
                    Price = p.Product.Price.ToString(),
                    Thumbnail = p.Product.Thumbnail,
                    ProductName = p.Product.ProductName,


                }).ToList();
                CurrentPage = currentPage;
                int skipCount = (CurrentPage - 1) * PageSize;
                int totalProducts = result.Count;
                TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);
                PagedProducts = PagedProducts.Skip(skipCount).Take(PageSize).ToList();
                return Page();
            }
          
            if (latest)
            {
                products = await _context.TblProducts.OrderByDescending(p=>p.AddedOn).ToListAsync();
                PagedProducts = products.Select(p => new SearchFilter
                {
                    Skucode = p.SkuCode,
                    Price = p.Price,
                    Thumbnail = p.Thumbnail,
                    ProductName = p.ProductName,

                }).ToList();
                CurrentPage = currentPage;
                int skipCount = (CurrentPage - 1) * PageSize;
                int totalProducts = products.Count;
                TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);
                PagedProducts = PagedProducts.Skip(skipCount).Take(PageSize).ToList();



                return Page();
            }
            if (ascedingprice)
            {
                HttpContext.Session.SetString("priceasc", ascedingprice.ToString());

                return RedirectToPage("productlist");
            }
            if (descendingprice)
            {
                HttpContext.Session.SetString("pricedesc", descendingprice.ToString());

                return RedirectToPage("productlist");
            }
            return RedirectToPage("productlist");

        }


        public async Task<IActionResult> OnPostAddToCart(int Qty, string MaterialName, string Size, string SkuCode,  string Price, string AddOn, bool buynow = false)
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




                return RedirectToPage("productlist");


            }
            catch (Exception ex)
            {
                return Page();
            }
        }
    }
}