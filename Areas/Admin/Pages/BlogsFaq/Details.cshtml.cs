using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.BlogsFaq
{
    public class DetailsModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public DetailsModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public BlogFaq BlogFaq { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogfaq = await _context.TblBlogsFaq.FirstOrDefaultAsync(m => m.Id == id);
            if (blogfaq == null)
            {
                return NotFound();
            }
            else
            {
                BlogFaq = blogfaq;
            }
            return Page();
        }
    }
}
