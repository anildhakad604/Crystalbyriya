using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using CrystalByRiya.Models;
using Astaberry.Helpers;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateCartQtyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UpdateCartQtyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Property to hold cart items
        public List<Item> Carts { get; set; }
        public List<Cart> Usercart { get; set; }=new List<Cart>();

        // API to get the total cart count
        [HttpGet("getCartCount")]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var email = HttpContext.Session.GetString("UserEmail");
                // Retrieve the cart from the session using the session helper
                Carts = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                Usercart = await _context.TblCarts.Where(e => e.UserEmail == email).ToListAsync();

                if (Carts != null && Carts.Count>1)
                {
                    var totalCount = Carts?.Count ?? 0;
                    // Return the total count as a JSON response
                    return Ok(new { count = totalCount });
                }
                else
                {
                    var totalCounts=Usercart?.Count ?? 0;
                    return Ok(new { count = totalCounts });
                }



            }
            catch (Exception ex)
            {
                // Log the exception if needed, then return a generic error message
                // You might want to use a logging framework such as Serilog or NLog here
                return StatusCode(500, "An error occurred while retrieving the order summary. Please try again later.");
            }
        }
        }

}

