using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.ReviewGalleries
{
    public class IndexModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public IndexModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ReviewGallery> ReviewGallery { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ReviewGallery = await _context.TblReviewGallery.ToListAsync();
        }
    }
}
