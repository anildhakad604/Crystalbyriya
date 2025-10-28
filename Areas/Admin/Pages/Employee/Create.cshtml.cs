using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CrystalByRiya.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrystalByRiya.Areas.Admin.Pages.Employee
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CrystalByRiya.Models.Employee NewEmployee { get; set; }

       

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
           

           

            _context.TblEmployee.Add(NewEmployee);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
