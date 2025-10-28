using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Pages
{
    public class blogModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public string Currenturl { get; private set; }
        public List<Blogs> Blogs { get; set; }
        public int TotalPages { get; private set; }
        public int CurrentPage { get; private set; }

        public blogModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(int page = 1, int pageSize = 6)
        {
            try
            {
                Currenturl = HttpContext.Request.GetDisplayUrl();
                int totalBlogs = await _context.TblBlogs.CountAsync(e => e.IsActive == true);
                TotalPages = (int)Math.Ceiling(totalBlogs / (double)pageSize);
                CurrentPage = page;

                Blogs = await _context.TblBlogs
                    .Where(e => e.IsActive == true)
                    .OrderByDescending(e => e.Blogid)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
            }
            return Page();
        }
    }
}
