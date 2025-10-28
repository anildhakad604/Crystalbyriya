using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.ProductFaqs
{
    public class CreateModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public CreateModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ProductFaq ProductFaq { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string LongDescription)
        {
            ProductFaq.Answer = LongDescription;

            _context.TblProductFaq.Add(ProductFaq);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
