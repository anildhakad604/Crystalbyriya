using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models; // Assuming the Employee model is under this namespace
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CrystalByRiya.Areas.Admin.Pages.Employee
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Ensure there is only one constructor
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CrystalByRiya.Models.Employee> Employees { get; set; }

        public async Task OnGetAsync()
        {
            Employees = await _context.TblEmployee.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var employee = await _context.TblEmployee.FindAsync(id);

            if (employee != null)
            {
                _context.TblEmployee.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
