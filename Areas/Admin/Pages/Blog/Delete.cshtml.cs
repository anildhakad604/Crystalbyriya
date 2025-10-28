using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;
using CrystalByRiya.Classes;

namespace CrystalByRiya.Areas.Admin.Pages.Blog
{
    public class DeleteModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        private readonly AmazonS3 _amazonS3;
        private readonly AwsCredentials awsCredentials;

        public DeleteModel(CrystalByRiya.Models.ApplicationDbContext context, AwsCredentials awsCredentials, AmazonS3 amazonS3)
        {
            _context = context;
            this.awsCredentials = awsCredentials;
            _amazonS3 = amazonS3;
        }

        [BindProperty]
        public Blogs Blogs { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _context.TblBlogs.FirstOrDefaultAsync(m => m.Blogid == id);

            if (blogs == null)
            {
                return NotFound();
            }
            else
            {
                Blogs = blogs;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _context.TblBlogs.FindAsync(id);
            if (blogs != null)
            {
                Blogs = blogs;
                await _amazonS3.DeleteFileFromS3(Blogs.Image);
                await _amazonS3.DeleteFileFromS3(Blogs.ThumbnailImage);
                _context.TblBlogs.Remove(Blogs);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
