using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.Products
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
        public int PageSize { get; set; } = 10;
        public IList<Product> Product { get; set; } = default;



        public async Task OnGetAsync(int currentPage = 1)
        {
            CurrentPage = currentPage;

            // Apply search filter if a search term is entered
            var productsQuery = _context.TblProducts.AsQueryable();
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(SearchTerm) || p.SkuCode.Contains(SearchTerm));
            }

            // Get total count for pagination
            var totalProducts = await productsQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

            // Fetch products with pagination
            Product = await productsQuery
                .OrderByDescending(p => p.AddedOn)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

    }
}













