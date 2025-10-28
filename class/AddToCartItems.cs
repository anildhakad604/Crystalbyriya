using Astaberry.Helpers;
using CrystalByRiya.Models;
using CrystalByRiya.Pages;
using CrystalByRiya.StoredProcedure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CrystalByRiya.@class
{
    public class AddToCartItems
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AddToCartItems> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AddToCartItems(ApplicationDbContext context, ILogger<AddToCartItems> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
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

       
        public Product Products { get; set; }
        public List<ChildskuCode> ChildskuCodes { get; set; } = new List<ChildskuCode>();
        public List<Item> Usercart { get; set; } = new List<Item>();
        public Cart CartMarketing { get; set; }

        public async Task<IActionResult> OnPostAddToCarts(int Qty, string MaterialName, string Size, string SkuCode, bool buynow, string Price, string Childsku, string AddOn,string useremail,string phone,string username)
        {
            try
            {
               

                // Fetch product details using stored procedure
                var skuParam = new SqlParameter("@PCode", SkuCode);
                var results = await _context.ProductBySkuCodes.FromSqlRaw("EXEC GetProductBySkuCode @PCode", skuParam).ToListAsync();
                var result = results.FirstOrDefault();

                if (result != null)
                {
                    // Initialize product details
                    ProductName = result.ProductName;
                    ParentProductCode = result.ParentCode;
                    ProductPrice = double.TryParse(Price, out double parsedPrice) ? parsedPrice : 0.0;
                    ProductGst = 3;
                    Gstaddon = ProductPrice / ProductGst;
                    TotalGst = ProductPrice - Gstaddon;
                    Netamount = ProductPrice - TotalGst;
                    ImgName = result.Thumbnail;
                    if (buynow)
                    {
                        var childskuItem = new ChildskuCode
                        {
                            Image = ImgName,
                            SKUCode = Childsku,
                            Price = ProductPrice,
                            ProductID = SkuCode,
                            Product = ProductName,
                            Quantity = Qty,
                            Gst = 3,
                            Material = string.IsNullOrEmpty(MaterialName) ? "NA" : MaterialName,
                            Size = string.IsNullOrEmpty(Size) ? "NA" : Size,
                            Addon = string.IsNullOrEmpty(AddOn) ? "NA" : AddOn
                        };

                        SessionHelper.SetObjectAsJson(_httpContextAccessor.HttpContext.Session, "buynowItem", new List<ChildskuCode> { childskuItem });
                        return new OkResult();
                    }
                    // Prepare the cart item
                    var item = new Item
                    {
                        Image = ImgName,
                        Price = ProductPrice,
                        ProductId = SkuCode,
                        ProductName = ProductName,
                        Qty = Qty,
                        Gst = ProductGst,
                        MaterialName = string.IsNullOrEmpty(MaterialName) ? "NA" : MaterialName,
                        Size = string.IsNullOrEmpty(Size) ? "NA" : Size,
                        Addon = string.IsNullOrEmpty(AddOn) ? "NA" : AddOn
                    };

                    string normalizedSize = Size ?? "NA";
                    string normalizedMaterial = MaterialName ?? "NA";
                    string normalizedAddon = AddOn ?? "NA";

                    // Check for existing cart item in the database
                    var existingCart = await _context.TblCarts.SingleOrDefaultAsync(e =>
                        e.ProductName == ProductName &&
                        e.UserEmail == useremail &&
                        e.size == normalizedSize &&
                        e.material == normalizedMaterial &&
                        e.addon == normalizedAddon);

                    if (existingCart == null)
                    {
                        CartMarketing = new Cart
                        {
                            ProductId = SkuCode,
                            ProductName = ProductName,
                            Image = ImgName,
                            Date = DateTime.Now,
                            Price = ProductPrice,
                            Qty = Qty,
                            size = Size ?? "NA",
                            material = MaterialName ?? "NA",
                            addon = AddOn ?? "NA",
                            UserEmail = useremail,
                            UserName = username,
                            Phone = phone,
                            Marketing = "No",
                            Gst = 3
                        };
                        if (useremail != null)
                        {
                            await _context.TblCarts.AddAsync(CartMarketing);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        existingCart.Qty += Qty;
                        int sum = existingCart.Qty;
                        Qty = sum;
                        _context.TblCarts.Update(existingCart);
                        await _context.SaveChangesAsync();
                    }
                    // Retrieve session cart
                    Usercart = SessionHelper.GetObjectFromJson<List<Item>>(_httpContextAccessor.HttpContext.Session,"cart") ?? new List<Item>();
                    if (Usercart == null)
                    {
                        Usercart = await _context.TblCarts
                            .Where(c => c.UserEmail == useremail)
                            .Select(c => new Item
                            {
                                ProductId = c.ProductId,
                                ProductName = c.ProductName,
                                Price = c.Price,
                                Qty = c.Qty,
                                Image = c.Image,
                                Gst = c.Gst,
                                MaterialName = c.material,
                                Size = c.size,
                                Addon = c.addon
                            })
                            .ToListAsync();
                    }

                    // Handle "Buy Now"
                   

                    // Handle regular cart logic
                    var existingItem = Usercart.FirstOrDefault(i => i.ProductId == SkuCode &&
                        (string.IsNullOrEmpty(Size) || i.Size == Size) &&
                        (string.IsNullOrEmpty(MaterialName) || i.MaterialName == MaterialName) &&
                        (string.IsNullOrEmpty(AddOn) || i.Addon == AddOn));

                    if (existingItem != null)
                    {
                        existingItem.Qty += Qty;
                        existingItem.Gst = TotalGst * existingItem.Qty;
                    }
                    else
                    {
                        Usercart.Add(item);
                    }

                    // Update session cart
                    SessionHelper.SetObjectAsJson(_httpContextAccessor.HttpContext.Session, "cart", Usercart);



                    _httpContextAccessor.HttpContext.Session.SetString("productid", SkuCode);
                    _httpContextAccessor.HttpContext.Session.SetInt32("quantity", Qty);
                    _httpContextAccessor.HttpContext.Session.SetString("childsku", Childsku);
                    return new OkResult();
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return new BadRequestResult();
            }
        }
    }
}
