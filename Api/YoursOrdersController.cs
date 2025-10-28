using Microsoft.AspNetCore.Mvc;
using CrystalByRiya.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using static CrystalByRiya.Pages.checkoutModel;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace CrystalByRiya.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class YourOrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public YourOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class CustomerOrderDetailsDTO
        {
            public string OrderCode { get; set; }
            public string SkuCode { get; set; }
            public int Qty { get; set; }
            public double Price { get; set; }
            public string Status { get; set; }
            public double Gst { get; set; }
            public string Material { get; set; }
            public string Size { get; set; }
            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public string Image { get; set; }
            public string Email { get; set; }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetOrderSummary(bool isBuyNow = false)
        {
            try
            {
          
                var loggedinemail = HttpContext.Session.GetString("UserEmail");
                var billingemail = await _context.TblBillingDetails
      .Where(e => e.Emailid == loggedinemail)
      .Select(e => e.Emailid)
      .FirstOrDefaultAsync();



                if (billingemail != loggedinemail)
                {
                    return Forbid("No Orders for this user.");
                }


                var customerOrderDetails = await _context.TblCustomerOrderDetails
                    .Where(rs => rs.Email == loggedinemail)
                    .Join(
                        _context.TblProducts,
                        order => order.ProductId,
                        product => product.SkuCode,
                        (order, product) => new CustomerOrderDetailsDTO
                        {
                            OrderCode = order.OrderCode,
                            SkuCode = order.SkuCode,
                            Qty = order.Qty,
                            Price = order.Price,
                            Status = order.Status,
                            Gst = order.Gst,
                            Material = order.Material,
                            Size = order.Size,
                            ProductId = order.ProductId,
                            ProductName = product.ProductName,
                            Image = product.Thumbnail
                        })
                    .ToListAsync();

                if (customerOrderDetails == null || !customerOrderDetails.Any())
                {
                    return NotFound("No orders found.");
                }

                // Calculate subtotal, shipping, and total directly in the response
                double subtotal = customerOrderDetails.Sum(i => i.Price * i.Qty);
                double shipping = subtotal < 3000 ? 80 : 0;
                double total = subtotal + shipping;

                var response = new
                {
                    Carts = customerOrderDetails,
                    Subtotal = subtotal,
                    Shipping = shipping,
                    Total = total
                };

                // Return the order summary with all fields included in JSON response
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, "An error occurred while retrieving the order summary. Please try again later.");
            }
        }

    }
}
