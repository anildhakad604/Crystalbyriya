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
    public class IndexModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public IndexModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Blogs> Blogs { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Blogs = await _context.TblBlogs.ToListAsync();
        }
    }
}
