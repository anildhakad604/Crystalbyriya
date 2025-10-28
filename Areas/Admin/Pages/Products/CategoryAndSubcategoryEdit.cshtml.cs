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
    public class CategoryandSubCategoryEditModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;
        public CategoryandSubCategoryEditModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TblCategory> Categories { get; set; }
        public List<Subcategory> SubCategories { get; set; }

        [BindProperty]
        public string SkuCode { get; set; }

        [BindProperty]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        [BindProperty]
        public List<int> SelectedSubCategoryIds { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync(string sku)
        {
            if (string.IsNullOrEmpty(sku))
            {
                return NotFound();
            }

            SkuCode = sku;

            Categories = await _context.TblCategory.ToListAsync();
            SubCategories = await _context.TblSubcategory.ToListAsync();

            var existingProducts = await _context.TblCategoryWiseProduct
                .Where(p => p.SkuCode == sku)
                .ToListAsync();

            SelectedCategoryIds = existingProducts.Select(p => p.CategoryId).Distinct().ToList();
            SelectedSubCategoryIds = existingProducts.Select(p => p.SubCategoryId).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(SkuCode) || !SelectedCategoryIds.Any() || !SelectedSubCategoryIds.Any())
            {
                ModelState.AddModelError(string.Empty, "All fields are required.");
                return Page();
            }

            // Remove existing entries
            var existingEntries = await _context.TblCategoryWiseProduct
                .Where(p => p.SkuCode == SkuCode)
                .ToListAsync();
            _context.TblCategoryWiseProduct.RemoveRange(existingEntries);

            // Add new entries based on selected categories and subcategories
            var categoryWiseProducts = new List<CategoryWiseProduct>();
            foreach (var categoryId in SelectedCategoryIds)
            {
                var validSubCategoryIds = await _context.TblSubcategory
                                                       .Where(sc => sc.CategoryId == categoryId)
                                                       .Select(sc => sc.SubCategoryid)
                                                       .ToListAsync();

                foreach (var subCategoryId in SelectedSubCategoryIds)
                {
                    if (validSubCategoryIds.Contains(subCategoryId))
                    {
                        categoryWiseProducts.Add(new CategoryWiseProduct
                        {
                            SkuCode = SkuCode,
                            CategoryId = categoryId,
                            SubCategoryId = subCategoryId
                        });
                    }
                }
            }

            _context.TblCategoryWiseProduct.AddRange(categoryWiseProducts);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
