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

namespace CrystalByRiya.Areas.Admin.Pages.Banners
{
    public class EditModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;
        private readonly AmazonS3 _amazonS3;
        private readonly AwsCredentials awsCredentials;

        public EditModel(CrystalByRiya.Models.ApplicationDbContext context, AwsCredentials awsCredentials,AmazonS3 amazonS3)
        {
            _context = context;
            this.awsCredentials = awsCredentials;
            _amazonS3 = amazonS3;
        }
        [BindProperty]
        public string hdndekstopimage { get; set; }
        [BindProperty]
        public string hdnmobileimage { get; set; }

        [BindProperty]
        public Banner Banners { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banners =  await _context.TblBanners.FirstOrDefaultAsync(m => m.Id == id);
            if (banners == null)
            {
                return NotFound();
            }
            Banners = banners;
            hdndekstopimage = banners.DesktopImageName;
            hdnmobileimage = banners.MobileImageName;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile dekstopimage,IFormFile mobileimage)
        {
           
          
                Banners.DesktopImageName = dekstopimage != null ? await _amazonS3.UploadFileToS3(dekstopimage, awsCredentials.BannerFoldername) : hdndekstopimage;
           
         
           
                Banners.MobileImageName= mobileimage != null?await _amazonS3.UploadFileToS3(mobileimage, awsCredentials.BannerFoldername): hdnmobileimage;
            
          


                _context.Attach(Banners).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BannersExists(Banners.Id))
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

        private bool BannersExists(int id)
        {
            return _context.TblBanners.Any(e => e.Id == id);
        }
    }
}
