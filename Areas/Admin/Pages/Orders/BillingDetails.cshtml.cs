using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CrystalByRiya.Areas.Admin.Pages.Orders
{
    public class BillingDetailsModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;
        public BillingDetailsModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<TblOrderId> Orders { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Orders = await _context.TblOrderIds.OrderByDescending(o => o.Date).ToListAsync();
            return Page();
        }

        [HttpPost]
        public async Task<IActionResult> OnPostUpdateStatusAsync([FromBody] UpdateStatusRequest request)
        {
            if (string.IsNullOrEmpty(request.OrderId) || string.IsNullOrEmpty(request.Field) || string.IsNullOrEmpty(request.Value))
            {
                return BadRequest("Invalid request data.");
            }

            // Find the order by ID
            var order = await _context.TblOrderIds.FirstOrDefaultAsync(o => o.Orderid == request.OrderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            // Update the specified field
            if (request.Field == "Status")
            {
                order.Status = request.Value;
            }
            else if (request.Field == "PaymentStatus")
            {
                order.PaymentStatus = request.Value;
            }
            else
            {
                return BadRequest("Invalid field specified.");
            }

            // Save the changes
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        public class UpdateStatusRequest
        {
            public string OrderId { get; set; }
            public string Field { get; set; }
            public string Value { get; set; }
        }
    }
}
