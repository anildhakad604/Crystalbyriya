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
    public class IndexModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public IndexModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 9;
        public IList<ImageGallery> ImageGallery { get; set; } = default;

        public async Task OnGetAsync(int currentPage = 1)
        {
            CurrentPage = currentPage;

            var galleryQuery = _context.TblImageGalleries.AsQueryable();
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                galleryQuery = galleryQuery.Where(i => i.Productid.Contains(SearchTerm) );
            }

            var totalImages = await galleryQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalImages / (double)PageSize);

            ImageGallery = await galleryQuery
                .OrderByDescending(i => i.Id)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
