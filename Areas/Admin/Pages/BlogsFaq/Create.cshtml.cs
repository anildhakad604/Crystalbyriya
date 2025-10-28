using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.BlogsFaq
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
        public BlogFaq BlogFaq { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TblBlogsFaq.Add(BlogFaq);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
