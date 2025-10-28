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
    public class AddToWishlistModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AddToWishlistModel> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddToWishlistModel(ApplicationDbContext context, ILogger<AddToWishlistModel> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnPostAddToWishlist(string SkuCode, string ProductName)
        {
            if (string.IsNullOrEmpty(SkuCode))
            {
                return new BadRequestResult();  // Return a BadRequest if SkuCode is missing
            }

            var Skucodes = new SqlParameter("@PCode", SkuCode);
            var result = await _context.ProductBySkuCodes
                .FromSqlRaw("EXEC GetProductBySkuCode @PCode", Skucodes)
                .ToListAsync();

            if (result != null && result.Count > 0)
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var Userwishlist = session.GetObjectFromJson<List<AddToWishlist>>("wishlist") ?? new List<AddToWishlist>();
                var useremail = session.GetString("UserEmail");
                var username = session.GetString("UserName");
                var phone = session.GetString("Phone");

                if (!Userwishlist.Any(x => x.ProductId == SkuCode))
                {
                    Userwishlist.Add(new AddToWishlist
                    {
                        ParentProductId = result.FirstOrDefault().ParentCode,
                        Image = result.FirstOrDefault().Thumbnail,
                        Price = result.FirstOrDefault().Price,
                        ProductId = SkuCode,
                        ProductName = ProductName
                    });
                }

                var check = await _context.TblWishlist.Where(e => e.UserEmail == useremail && e.ProductName == ProductName).FirstOrDefaultAsync();
                if (check == null)
                {
                    var newWishlistItem = new AddingWishlist
                    {
                        Phone = phone,
                        Marketing = "No",
                        Price = result.FirstOrDefault().Price,
                        UserEmail = useremail,
                        UserName = username,
                        ProductName = ProductName,
                        Image = result.FirstOrDefault().Thumbnail,
                        Date = DateTime.Now
                    };
                    try
                    {
                        if (useremail != null)
                        {
                            await _context.TblWishlist.AddAsync(newWishlistItem);
                            await _context.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saving changes to TblWishlist.");
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError); // Return server error if saving fails
                    }

                    // Add the new wishlist item to the session
                    Userwishlist.Add(new AddToWishlist
                    {
                        ParentProductId = result.FirstOrDefault().ParentCode,
                        Image = result.FirstOrDefault().Thumbnail,
                        Price = result.FirstOrDefault().Price,
                        ProductId = SkuCode,
                        ProductName = ProductName
                    });
                }

                session.SetObjectAsJson("wishlist", Userwishlist);
                _httpContextAccessor.HttpContext.Session.SetString("WishlistStatus", "true");

                return new OkResult();  // Return 200 OK when the product is added successfully
            }
            else
            {
                return new BadRequestResult();  // Return BadRequest if the product is not found
            }
        }
    }
}
