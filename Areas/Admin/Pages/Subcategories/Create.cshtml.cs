using CrystalByRiya.Classes;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CrystalByRiya.Areas.Admin.Pages.Subcategories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly AmazonS3 _amazonS3;
        private readonly AwsCredentials awsCredentials;

        public CreateModel(CrystalByRiya.Models.ApplicationDbContext context, AwsCredentials awsCredentials, AmazonS3 amazonS3)
        {
            _context = context;
            this.awsCredentials = awsCredentials;
            _amazonS3 = amazonS3;
        }

        [BindProperty]
        public Subcategory Subcategory { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync( IFormFile CategoryImage, IFormFile ThumbnailImage)
        {

            Subcategory.MetaTitle = "NA";
            Subcategory.Categoryimage = await _amazonS3.UploadFileToS3(CategoryImage, awsCredentials.SubCategoryFoldername);
            Subcategory.ThumbnailImage = await _amazonS3.UploadFileToS3(ThumbnailImage, awsCredentials.SubCategoryFoldername);

            _context.TblSubcategory.Add(Subcategory);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
