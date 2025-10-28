using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.ProductFaqs
{
    public class DeleteModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public DeleteModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductFaq ProductFaq { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productfaq = await _context.TblProductFaq.FirstOrDefaultAsync(m => m.Id == id);

            if (productfaq == null)
            {
                return NotFound();
            }
            else
            {
                ProductFaq = productfaq;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productfaq = await _context.TblProductFaq.FindAsync(id);
            if (productfaq != null)
            {
                ProductFaq = productfaq;
                _context.TblProductFaq.Remove(ProductFaq);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
