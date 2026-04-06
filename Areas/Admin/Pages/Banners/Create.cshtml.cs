using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CrystalByRiya.Models;
using CrystalByRiya.Classes;
using CrystalByRiya.Classes;
using System.Linq.Expressions;

namespace CrystalByRiya.Areas.Admin.Pages.Banners
{
    public class CreateModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;
        private readonly AwsCredentials _awsCredentials;
        private readonly AmazonS3 _amazonS3;
        IWebHostEnvironment _Env;
        public CreateModel(CrystalByRiya.Models.ApplicationDbContext context,AwsCredentials awsCredentials,AmazonS3 amazonS3,IWebHostEnvironment Env) 
        {
            _context = context;
            _awsCredentials = awsCredentials;
            _amazonS3 = amazonS3;
            _Env = Env;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Banner Banners { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(IFormFile MobileImage, IFormFile DekstopImage)
        {
           
            //if (MobileImage != null)
            //{
            //    Banners.MobileImageName = await _amazonS3.UploadFileToS3(MobileImage, _awsCredentials.BannerFoldername);
            //}
            //else
            //{
            //    Banners.MobileImageName = "NA";
            //}
            //if (DekstopImage != null)
            //{
            //    Banners.DesktopImageName = await _amazonS3.UploadFileToS3(DekstopImage, _awsCredentials.BannerFoldername);
            //}
            //else
            //{
            //    Banners.DesktopImageName = "NA";
            //}
            



            _context.TblBanners.Add(Banners);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
