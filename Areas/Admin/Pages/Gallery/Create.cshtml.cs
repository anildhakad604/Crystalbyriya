using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CrystalByRiya.Models;
using CrystalByRiya.Classes;

namespace CrystalByRiya.Areas.Admin.Pages.Gallery
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
        public ImageGallery ImageGallery { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(List<IFormFile> Url)
        {

            foreach (var uploadedFile in Url)
            {
                if (uploadedFile.Length > 1024 * 1024) // 1 MB in bytes
                {
                    ModelState.AddModelError("Product.Thumbnail", "The thumbnail size must not exceed 1 MB.");
                    return Page(); // Return to the page with the validation error
                }
                // Create a unique file name to avoid conflicts
                string fileName = await _amazonS3.UploadFileToS3(uploadedFile, awsCredentials.GalleryFoldername);
               

                // Save the image details to the database
                ImageGallery image = new ImageGallery()
                {
                    Productid = ImageGallery.Productid,
                    Url = fileName, // Save the filename, not the full path
                    Type = ImageGallery.Type,
                    AltText=ImageGallery.AltText
                   
                    
                };

                await  _context.TblImageGalleries.AddAsync(image);
                await _context.SaveChangesAsync();

            }

            return RedirectToPage("./Index");
        }
    }
}
