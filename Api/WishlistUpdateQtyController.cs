using Astaberry.Helpers;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistUpdateQtyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public WishlistUpdateQtyController(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<AddToWishlist> Wishlist { get; set; }
        public List<AddingWishlist> wishlists { get; set; }
        [HttpGet("getWishlistCount")]
        public async Task<IActionResult> GetWishlistCount()
        {
            try
            {
                var email = HttpContext.Session.GetString("UserEmail");
                Wishlist = SessionHelper.GetObjectFromJson<List<AddToWishlist>>(HttpContext.Session, "wishlist");
                wishlists = await _context.TblWishlist.Where(e => e.UserEmail == email).ToListAsync();
                if (Wishlist != null && Wishlist.Count>1)
                {
                    var wishlistcount = Wishlist?.Count ?? 0;
                    return Ok(new { Count = wishlistcount });
                }
                else
                {
                    var wishlistcounts = wishlists?.Count ?? 0;
                    return Ok(new { Count = wishlistcounts });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
