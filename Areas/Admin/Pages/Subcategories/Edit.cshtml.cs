using CrystalByRiya.Classes;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CrystalByRiya.Areas.Admin.Pages.Subcategories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly AmazonS3 _amazonS3;
        private readonly AwsCredentials awsCredentials;

        public EditModel(CrystalByRiya.Models.ApplicationDbContext context, AwsCredentials awsCredentials, AmazonS3 amazonS3)
        {
            _context = context;
            this.awsCredentials = awsCredentials;
            _amazonS3 = amazonS3;
        }
        [BindProperty]
        public string hdnthumbnailimage{ get; set; }
        [BindProperty]
        public string hdncategoryimage { get; set; }

        [BindProperty]
        public Subcategory Subcategory { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Subcategory = await _context.TblSubcategory.FindAsync(id);

            if (Subcategory == null)
            {
                return NotFound();
            }
            hdncategoryimage = Subcategory.Categoryimage;

            hdnthumbnailimage = Subcategory.ThumbnailImage;
            return Page();
        }

       
        public async Task<IActionResult> OnPostAsync(string title,IFormFile CategoryImage,IFormFile ThumbnailImage)
        {
            Subcategory.MetaTitle = title;
            Subcategory.Categoryimage=CategoryImage!=null?await _amazonS3.UploadFileToS3(CategoryImage,awsCredentials.SubCategoryFoldername):hdncategoryimage;
            Subcategory.ThumbnailImage= ThumbnailImage != null ? await _amazonS3.UploadFileToS3(ThumbnailImage, awsCredentials.SubCategoryFoldername) : hdnthumbnailimage;

            _context.Attach(Subcategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubcategoryExists(Subcategory.SubCategoryid))
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

        private bool SubcategoryExists(int id)
        {
            return _context.TblSubcategory.Any(e => e.SubCategoryid == id);
        }
    }
}