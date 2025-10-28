using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CrystalByRiya.Models;
using CrystalByRiya.Classes;


namespace CrystalByRiya.Areas.Admin.Pages.Blog
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
        [BindProperty]
        public string hdndekstopimage { get; set; }
        [BindProperty]
        public string hdnmobileimage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Blogs Blogs { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync( string title,IFormFile Image,IFormFile ThumbnailImage,string editor)
        {
           
            Blogs.MetaTitle = title;
            Blogs.Blogdescription = editor;
            Blogs.Image = await _amazonS3.UploadFileToS3(Image, awsCredentials.BlogFoldername);
            Blogs.ThumbnailImage = await _amazonS3.UploadFileToS3(ThumbnailImage, awsCredentials.BlogFoldername);
            _context.TblBlogs.Add(Blogs);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
