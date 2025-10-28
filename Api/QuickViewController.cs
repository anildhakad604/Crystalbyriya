using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using CrystalByRiya.Models;
using Microsoft.CodeAnalysis;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace CrystalByRiya.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuickViewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuickViewController> _logger;

        public QuickViewController(ApplicationDbContext context, ILogger<QuickViewController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        [Route("GetQuickView")]
        public async Task<IActionResult> GetQuickView(string skucode)
        {
            if (string.IsNullOrEmpty(skucode))
            {
                _logger.LogWarning("SkuCode is missing in the request.");
                return BadRequest(new { success = false, message = "SkuCode is required." });
            }

            try
            {
                var Sizes = await _context.TblProductSizes
      .Where(e => e.ProductID == skucode)
      .Select(e => $"{e.Size}:{e.Price}:{e.ImageURL}") // Combine Size and Price into a single string
      .ToListAsync();

                var Addons = await _context.TblAddOn
                    .Where(e => e.ProductId == skucode)
                    .Select(e => $"{e.AddOnName}:{e.Price}") // Combine AddOnName and Price into a single string
                    .ToListAsync();

                var materials = await _context.Materials
                    .Where(e => e.SkuCode == skucode)
                    .Select(m => $"{m.MaterialName}:{m.Price}:{m.Image}") // Combine MaterialName and Price into a single string
                    .ToListAsync();
                var Images = await _context.TblImageGalleries.Where(e => e.Productid == skucode).Select(e => e.Url).ToListAsync();
                // Prepare parameter for stored procedure
                var skucodeParam = new SqlParameter("@PCode", skucode);

                // Execute stored procedure to fetch product details
                var productResult = await _context.ProductBySkuCodes
                    .FromSqlRaw("EXEC GetProductBySkuCode @PCode", skucodeParam)
                    .ToListAsync();

                var result = productResult.FirstOrDefault();
                if (result == null)
                {
                    _logger.LogWarning($"No product found for SkuCode: {skucode}");
                    return NotFound(new { success = false, message = "Product not found." });
                }

                var quickViewResponse =new QuickViews
                {
                    ParentProductId = result.ParentCode,
                    Image = Images,
                    Price = result.Price,
                    ProductId = skucode,
                    ProductName = result.ProductName,
                   
                    Description = result.ProductDescription,
                    Tags=result.keywords,
                    Size=Sizes,
                    AddOn=Addons,
                    MaterialName=materials
                   
                };

                // Return the QuickView item as a JSON result
                return Ok(new { success = true, data = quickViewResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the QuickView request.");
                return StatusCode(500, new { success = false, message = "An error occurred. Please try again later." });
            }
        }
    }
}
