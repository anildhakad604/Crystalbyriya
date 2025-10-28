using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;
using CrystalByRiya.Classes;

namespace CrystalByRiya.Areas.Admin.Pages.Gallery
{
    public class EditModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        private readonly AmazonS3 _amazonS3;
        private readonly AwsCredentials awsCredentials;

        public EditModel(CrystalByRiya.Models.ApplicationDbContext context, AwsCredentials awsCredentials, AmazonS3 amazonS3)
        {
            _context = context;
            this.awsCredentials = awsCredentials;
            _amazonS3 = amazonS3;
        }

        [BindProperty]
        public ImageGallery ImageGallery { get; set; } = default!;
        public string imgname { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imagegallery =  await _context.TblImageGalleries.FirstOrDefaultAsync(m => m.Id == id);
            imgname = imagegallery.Url;
            if (imagegallery == null)
            {
                return NotFound();
            }
            ImageGallery = imagegallery;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(List<IFormFile> Url,string hdnUrl)
        {
            if (Url.Count >0)
            {


                foreach (var uploadedFile in Url)
                {
                    if (uploadedFile.Length > 1024 * 1024) 
                    {
                        ModelState.AddModelError("Product.Thumbnail", "The thumbnail size must not exceed 1 MB.");
                        return Page(); // Return to the page with the validation error
                    }
                    string fileName = await _amazonS3.UploadFileToS3(uploadedFile, awsCredentials.GalleryFoldername);
                

                    // Update the ImageGallery model with the new URL
                    ImageGallery.Url = fileName; // Save only the file name
                }
            }
            else
            {
                ImageGallery.Url = hdnUrl;
            }
        _context.Attach(ImageGallery).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageGalleryExists(ImageGallery.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ImageGalleryExists(int id)
        {
            return _context.TblImageGalleries.Any(e => e.Id == id);
        }
    }
}
