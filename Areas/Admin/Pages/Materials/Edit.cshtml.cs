using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Hosting;
using CrystalByRiya.Classes;

namespace CrystalByRiya.Areas.Admin.Pages.Materials
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
        public Material Material { get; set; } = default!;
        public string imagename { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material =  await _context.Materials.FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
            {
                return NotFound();
            }
            Material = material;
            imagename = Material.Image;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile ImageUrl, string hdnimageurl)
        {
            if (ImageUrl != null && ImageUrl.Length > 1024 * 1024) // 1 MB in bytes
            {
                ModelState.AddModelError("Product.Thumbnail", "The thumbnail size must not exceed 1 MB.");
                return Page(); // Return to the page with the validation error
            }

            if (ImageUrl != null)
            {
                string fileName = await _amazonS3.UploadFileToS3(ImageUrl, awsCredentials.MaterialFoldername);
                Material.Image = fileName;
            }
            else
            {
                Material.Image = hdnimageurl;
            }
            _context.Attach(Material).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialExists(Material.Id))
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

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.Id == id);
        }
    }
}
