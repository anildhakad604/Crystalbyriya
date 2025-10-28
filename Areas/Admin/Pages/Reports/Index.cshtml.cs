using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CrystalByRiya.Areas.Admin.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public int TotalOrders { get; private set; }
        public double TotalSales { get; private set; }
        public int TotalRegisteredUsers { get; private set; }

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Calculate total orders
            TotalOrders = await _context.TblOrderIds.CountAsync();

            // Calculate total sales amount for completed orders
            TotalSales = await _context.TblOrderIds
                                      .Where(o => o.Status == "Completed")
                                      .SumAsync(o => o.TotalAmount);

            // Calculate total registered users
            TotalRegisteredUsers = await _context.TblRegisters.CountAsync();

            return Page();
        }
    }
}
