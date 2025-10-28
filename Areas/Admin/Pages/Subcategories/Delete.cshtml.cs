using CrystalByRiya.Classes;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CrystalByRiya.Areas.Admin.Pages.Subcategories
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly AmazonS3 _amazonS3;
        private readonly AwsCredentials awsCredentials;

        public DeleteModel(CrystalByRiya.Models.ApplicationDbContext context, AwsCredentials awsCredentials, AmazonS3 amazonS3)
        {
            _context = context;
            this.awsCredentials = awsCredentials;
            _amazonS3 = amazonS3;
        }

        [BindProperty]
        public Subcategory Subcategory { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Subcategory = await _context.TblSubcategory.FindAsync(id);

            if (Subcategory == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var subcategory = await _context.TblSubcategory.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }
            await _amazonS3.DeleteFileFromS3(subcategory.ThumbnailImage);
            await _amazonS3.DeleteFileFromS3(subcategory.Categoryimage);

            _context.TblSubcategory.Remove(subcategory);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
