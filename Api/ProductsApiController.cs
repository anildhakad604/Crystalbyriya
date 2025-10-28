using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CrystalByRiya.Models;

namespace CrystalByRiya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Ok(new string[] { }); // Return an empty array if no term provided

            var suggestions = await _context.TblProducts
                .Where(p => p.ProductName.StartsWith(searchTerm) || p.SkuCode.StartsWith(searchTerm))
                .Select(p => new { p.ProductName, p.SkuCode })
                .Take(10) // Limit to 10 suggestions
                .ToListAsync();

            return Ok(suggestions);
        }
    }
}
