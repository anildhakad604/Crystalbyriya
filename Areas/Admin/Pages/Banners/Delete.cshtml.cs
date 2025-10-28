using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;
using CrystalByRiya.Classes;

namespace CrystalByRiya.Areas.Admin.Pages.Banners
{
    public class DeleteModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        private readonly AwsCredentials _awsCredentials;
        private readonly AmazonS3 _amazonS3;
        public DeleteModel(CrystalByRiya.Models.ApplicationDbContext context, AwsCredentials awsCredentials, AmazonS3 amazonS3)
        {
            _context = context;
            _awsCredentials = awsCredentials;
            _amazonS3 = amazonS3;
        }

        [BindProperty]
        public Banner Banners { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banners = await _context.TblBanners.FirstOrDefaultAsync(m => m.Id == id);

            if (banners == null)
            {
                return NotFound();
            }
            else
            {
                Banners = banners;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banners = await _context.TblBanners.FindAsync(id);
            if (banners != null)
            {
                Banners = banners;
                await _amazonS3.DeleteFileFromS3(Banners.DesktopImageName);
                await _amazonS3.DeleteFileFromS3(Banners.MobileImageName);
                _context.TblBanners.Remove(Banners);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
