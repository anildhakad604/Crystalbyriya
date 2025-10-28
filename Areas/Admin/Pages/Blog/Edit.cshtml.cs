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

namespace CrystalByRiya.Areas.Admin.Pages.Blog
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
        public string hdnimage { get; set; }
        [BindProperty]
        public string hdnthumbnailimage { get; set; }
        [BindProperty]
        public Blogs Blogs { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs =  await _context.TblBlogs.FirstOrDefaultAsync(m => m.Blogid == id);
            if (blogs == null)
            {
                return NotFound();
            }
            Blogs = blogs;
            hdnimage = Blogs.Image;
            hdnthumbnailimage = Blogs.ThumbnailImage;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string title, IFormFile Image,IFormFile ThumbnailImage)
        {

            Blogs.MetaTitle = title;
            Blogs.Image = Image!=null? await _amazonS3.UploadFileToS3(Image, awsCredentials.BlogFoldername):hdnimage;
            Blogs.ThumbnailImage =ThumbnailImage!=null? await _amazonS3.UploadFileToS3(ThumbnailImage, awsCredentials.BlogFoldername):hdnthumbnailimage;
            _context.Attach(Blogs).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogsExists(Blogs.Blogid))
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

        private bool BlogsExists(int id)
        {
            return _context.TblBlogs.Any(e => e.Blogid == id);
        }
    }
}
