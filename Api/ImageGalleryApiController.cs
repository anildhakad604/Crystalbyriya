using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CrystalByRiya.Models;

namespace CrystalByRiya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageGalleryApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ImageGalleryApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Ok(new string[] { });

            var suggestions = await _context.TblImageGalleries
                .Where(i => i.Productid.StartsWith(searchTerm) )
                .Select(i => new { i.Productid, i.Type })
                .Take(10)
                .ToListAsync();

            return Ok(suggestions);
        }
    }
}
