using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CrystalByRiya.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace CrystalByRiya.Areas.Admin.Pages.Products
{
    public class GetInfoRequest
    {
        public string SkuCode { get; set; }
    }

    public class CategoryandSubCategoryModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;
        public CategoryandSubCategoryModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }
        public List<TblCategory> Categories { get; set; }
        public List<Subcategory> SubCategories { get; set; }
        [BindProperty]
        public CategoryWiseProduct CategoryWiseProduct { get; set; }

        [BindProperty]
        public string SkuCode { get; set; }
        
        [BindProperty]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        [BindProperty]
        public List<int> SelectedSubCategoryIds { get; set; } = new List<int>();

        public async Task OnGetAsync()
        {
            Categories = await _context.TblCategory.ToListAsync();
            SubCategories = await _context.TblSubcategory.ToListAsync();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(SkuCode) )
            {
                ModelState.AddModelError(string.Empty, "All fields are required.");
                return Page();
            }

            // Fetch existing CategoryWiseProduct entries for the given SkuCode
            var existingCategoryWiseProducts = await _context.TblCategoryWiseProduct
                .Where(cwp => cwp.SkuCode == SkuCode)
                .ToListAsync();

            // Create lists to track new and removed entries
            var newCategoryWiseProducts = new List<CategoryWiseProduct>();
            var removedCategoryWiseProducts = new List<CategoryWiseProduct>();

            // Iterate over selected categories and subcategories (checkboxes)
            foreach (var categoryId in SelectedCategoryIds)
            {
                // Retrieve valid SubCategoryIds for the current CategoryId
                var validSubCategoryIds = await _context.TblSubcategory
                                                        .Where(sc => sc.CategoryId == categoryId)
                                                        .Select(sc => sc.SubCategoryid)
                                                        .ToListAsync();

                foreach (var subCategoryId in SelectedSubCategoryIds)
                {
                    if (validSubCategoryIds.Contains(subCategoryId))
                    {
                        // Check if this category and subcategory combination already exists for the given SKU
                        var existingProduct = existingCategoryWiseProducts
                            .FirstOrDefault(cwp => cwp.CategoryId == categoryId && cwp.SubCategoryId == subCategoryId);

                        if (existingProduct == null)
                        {
                            // If not found, this is a new entry, so add it to the newCategoryWiseProducts list
                            var categoryWiseProduct = new CategoryWiseProduct
                            {
                                SkuCode = SkuCode,
                                CategoryId = categoryId,
                                SubCategoryId = subCategoryId
                            };
                            newCategoryWiseProducts.Add(categoryWiseProduct);
                        }
                    }
                }
            }

            // Identify and remove any CategoryWiseProduct entries that are no longer checked
            foreach (var existingProduct in existingCategoryWiseProducts)
            {
                // If the existing product's category and subcategory are not in the selected lists, remove it
                if (!SelectedCategoryIds.Contains(existingProduct.CategoryId) ||
                    !SelectedSubCategoryIds.Contains(existingProduct.SubCategoryId))
                {
                    removedCategoryWiseProducts.Add(existingProduct);
                }
            }

            // Perform database updates
            if (newCategoryWiseProducts.Any())
            {
                // Add new category and subcategory mappings
                _context.TblCategoryWiseProduct.AddRange(newCategoryWiseProducts);
            }

            if (removedCategoryWiseProducts.Any())
            {
                // Remove the unchecked category and subcategory mappings
                _context.TblCategoryWiseProduct.RemoveRange(removedCategoryWiseProducts);
            }

            // Save all changes to the database
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

    }
} 