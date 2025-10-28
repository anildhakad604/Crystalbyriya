using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.Banners
{
    public class DetailsModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public DetailsModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public Banner Banners { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banners = await _context.TblBanners.FirstOrDefaultAsync(m => m.Id == id);
            if (banners == null)
            {
                return NotFound();
            }
            else
            {
                Banners = banners;
            }
            return Page();
        }
    }
}
