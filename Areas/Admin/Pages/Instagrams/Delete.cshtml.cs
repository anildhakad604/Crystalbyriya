using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.Instagrams
{
    public class DeleteModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public DeleteModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Instagram Instagram { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instagram = await _context.TblInstagram.FirstOrDefaultAsync(m => m.Id == id);

            if (instagram == null)
            {
                return NotFound();
            }
            else
            {
                Instagram = instagram;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instagram = await _context.TblInstagram.FindAsync(id);
            if (instagram != null)
            {
                Instagram = instagram;
                _context.TblInstagram.Remove(Instagram);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
