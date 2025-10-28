using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CrystalByRiya.Models;

namespace CrystalByRiya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSizesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductSizesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Ok(new string[] { }); // Return an empty array if no term is provided

            var suggestions = await _context.TblProductSizes
                .Where(p => p.ProductID.StartsWith(searchTerm) || p.SKUCode.StartsWith(searchTerm))
                .Select(p => new { p.ProductID, p.SKUCode })
                .Take(10) // Limit to 10 suggestions
                .ToListAsync();

            return Ok(suggestions);
        }
    }
}
