using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CrystalByRiya.Models;
using CrystalByRiya.Classes;

namespace CrystalByRiya.Areas.Admin.Pages.Materials
{
    public class CreateModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        private readonly AmazonS3 _amazonS3;
        private readonly AwsCredentials awsCredentials;

        public CreateModel(CrystalByRiya.Models.ApplicationDbContext context, AwsCredentials awsCredentials, AmazonS3 amazonS3)
        {
            _context = context;
            this.awsCredentials = awsCredentials;
            _amazonS3 = amazonS3;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Material Material { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile ImageUrl)
        {
            if (ImageUrl != null && ImageUrl.Length > 1024 * 1024) // 1 MB in bytes
            {
                ModelState.AddModelError("Product.Thumbnail", "The thumbnail size must not exceed 1 MB.");
                return Page(); // Return to the page with the validation error
            }

          
           
            string fileName = await _amazonS3.UploadFileToS3(ImageUrl, awsCredentials.MaterialFoldername);
            Material.Image = fileName;
            await _context.Materials.AddAsync(Material);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
