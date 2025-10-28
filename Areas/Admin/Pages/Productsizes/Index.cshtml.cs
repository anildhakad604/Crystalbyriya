using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
namespace CrystalByRiya.Areas.Admin.Pages.Productsizes
{

    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public bool ShowLowStock { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public IList<ProductSizes> ProductSizes { get; set; } = default!;

        public async Task OnGetAsync(int currentPage = 1)
        {
            CurrentPage = currentPage;

            // Apply search filter
            var productSizesQuery = _context.TblProductSizes.AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                productSizesQuery = productSizesQuery.Where(p =>
                    p.ProductID.Contains(SearchTerm) || p.SKUCode.Contains(SearchTerm));
            }

            // Filter for low stock products if ShowLowStock is true
            if (ShowLowStock)
            {
                productSizesQuery = productSizesQuery.Where(p => p.StockQuantity < 5);
            }

            // Total count for pagination
            var totalProductSizes = await productSizesQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalProductSizes / (double)PageSize);

            // Fetch data with pagination
            ProductSizes = await productSizesQuery
                .OrderByDescending(p => p.ProductID)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        // Method to call the stored procedure and update stock
        public async Task<IActionResult> OnPostUpdateStockAsync(string skuCode)
        {
            if (!string.IsNullOrEmpty(skuCode))
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"EXEC UpdateAvailableStock @SkuCode = {skuCode}");
            }

            return RedirectToPage();
        }
    }
}