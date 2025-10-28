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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using CrystalByRiya.Classes;

namespace CrystalByRiya.Areas.Admin.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;
     
        public IList<SelectListItem> Category { get; set; }             
        public IList<SelectListItem> SubCategory { get; set; }

        private readonly AmazonS3 _amazonS3;
        private readonly AwsCredentials awsCredentials;

        public CreateModel(CrystalByRiya.Models.ApplicationDbContext context, AwsCredentials awsCredentials, AmazonS3 amazonS3)
        {
            _context = context;
            this.awsCredentials = awsCredentials;
            _amazonS3 = amazonS3;
        }

        public async Task<IActionResult> OnGet()
        {
            Category = await _context.TblCategory.Select(c => new SelectListItem { Text = c.MenuList, Value = c.Id.ToString() }).ToListAsync();
            SubCategory = await _context.TblSubcategory.Select(c => new SelectListItem { Text = c.SubCategoryname, Value = c.SubCategoryid.ToString() }).ToListAsync();
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string LongDescription,string ParentCode,IFormFile thumbnail ,string title )
        {
            if (thumbnail != null && thumbnail.Length > 1024 * 1024) // 1 MB in bytes
            {
                ModelState.AddModelError("Product.Thumbnail", "The thumbnail size must not exceed 1 MB.");
                return Page(); // Return to the page with the validation error
            }
            string fileName = await _amazonS3.UploadFileToS3(thumbnail, awsCredentials.ProductsFoldername);

            // Save thumbnail details in the database


            Product.ProductDescription = LongDescription;
            Product.ParentCode = ParentCode;
            Product.Thumbnail = fileName;
            Product.MetaTitle = title;
            await _context.TblProducts.AddAsync(Product);
            await _context.SaveChangesAsync();

           
            return RedirectToPage("./Index");
        }
    }
}
