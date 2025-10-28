using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.Gallery
{
    public class DetailsModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public DetailsModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public ImageGallery ImageGallery { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imagegallery = await _context.TblImageGalleries.FirstOrDefaultAsync(m => m.Id == id);
            if (imagegallery == null)
            {
                return NotFound();
            }
            else
            {
                ImageGallery = imagegallery;
            }
            return Page();
        }
    }
}
