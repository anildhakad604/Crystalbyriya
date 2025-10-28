using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public DetailsModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public Blogs Blogs { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _context.TblBlogs.FirstOrDefaultAsync(m => m.Blogid == id);
            if (blogs == null)
            {
                return NotFound();
            }
            else
            {
                Blogs = blogs;
            }
            return Page();
        }
    }
}
