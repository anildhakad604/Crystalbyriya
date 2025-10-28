using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogQuickViewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BlogQuickViewController(ApplicationDbContext context)
        {

            _context = context;
        }
        public Blogs blog { get; set; }
        public List<string> Skucodes { get; set; }
        public List<BlogQuickView> Blogsquickview { get; set; }
        public List<Product> Products { get; set; }



        [HttpGet]
        [Route("GetBlogQuickviews")]
        public async Task<IActionResult> GetBlogQuickviews(int blogid)
        {
            blog = await _context.TblBlogs.Where(e => e.Blogid == blogid).FirstOrDefaultAsync();
            var Skucodes = await _context.RelatedProducts.Where(e => e.BlogId == blogid).Select(e => e.Skucode).ToListAsync();
            var products = await _context.TblProducts
               .Where(p => Skucodes.Contains(p.SkuCode))
               .Select(p => new
               {
                   p.ProductName,
                   p.SkuCode,
                   p.Price,
                   p.Thumbnail
               })
               .ToListAsync();
            var blogQuickView = new BlogQuickView
            {
                blogid = blog.Blogid.ToString(),
                BlogTitle = blog.BlogTitle,
                Blogdescription = blog.Blogdescription,
                Image = blog.Image,
                Skucode = Skucodes,
                productname = products.Select(p => $"{p.ProductName}:{p.SkuCode}:{p.Price}:{p.Thumbnail}").ToList()
            };

            // Return the BlogQuickView as a response
            return Ok(new { success = true, data = blogQuickView });

        }
    }
}