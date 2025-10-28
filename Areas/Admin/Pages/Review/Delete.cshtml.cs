using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.Review
{
    public class DeleteModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public DeleteModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TblReviews TblReviews { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblreviews = await _context.TblReviews.FirstOrDefaultAsync(m => m.Id == id);

            if (tblreviews is not null)
            {
                TblReviews = tblreviews;

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

            var tblreviews = await _context.TblReviews.FindAsync(id);
            if (tblreviews != null)
            {
                TblReviews = tblreviews;
                _context.TblReviews.Remove(TblReviews);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
