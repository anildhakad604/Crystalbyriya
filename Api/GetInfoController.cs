using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetInfoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetInfo([FromQuery] string skuCode)
        {
            try {
                if (string.IsNullOrEmpty(skuCode))
                {
                    return BadRequest("SKU code is required.");
                }

                // Fetch the related categories and subcategories based on SKU code
                var categoryWiseProducts = await _context.TblCategoryWiseProduct
                    .Where(cwp => cwp.SkuCode == skuCode)
                    .ToListAsync();

                if (!categoryWiseProducts.Any())
                {
                    return NotFound("No categories and subcategories found for this SKU code.");
                }

                // Get the selected category and subcategory IDs
                var selectedCategories = categoryWiseProducts
                    .Select(cwp => cwp.CategoryId)
                    .Distinct()
                    .ToList();

                var selectedSubCategories = categoryWiseProducts
                    .Select(cwp => cwp.SubCategoryId)
                    .Distinct()
                    .ToList();

                return Ok(new
                {
                    selectedCategories,
                    selectedSubCategories
                });
            }
            catch (Exception ex)
            {
                // Log the exception if needed, then return a generic error message
                // You might want to use a logging framework such as Serilog or NLog here
                return StatusCode(500, "An error occurred while retrieving the order summary. Please try again later.");
            }
        }

    }
}
