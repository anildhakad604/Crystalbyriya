using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.Category
{
    public class DeleteModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public DeleteModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TblCategory TblCategory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblcategory = await _context.TblCategory.FirstOrDefaultAsync(m => m.Id == id);

            if (tblcategory is not null)
            {
                TblCategory = tblcategory;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblcategory = await _context.TblCategory.FindAsync(id);
            if (tblcategory != null)
            {
                TblCategory = tblcategory;
                _context.TblCategory.Remove(TblCategory);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
