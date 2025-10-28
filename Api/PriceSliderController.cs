using CrystalByRiya.Models;
using CrystalByRiya.StoredProcedure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PriceSliderController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PriceSliderController(ApplicationDbContext context)
    {
        _context = context;
    }
    public Subcategory Subcategory { get; set; }
    public TblCategory Category { get; set; }
    public List<Subcategory> SubCategoryList { get; set; }
    public List<SPSubCategoryWiseProduct> SPSubCategoryWiseProduct { get; set; }
    [HttpGet("GetProductsByPriceRange")]
    public async Task<IActionResult> GetProductsByPriceRange(string catname, string subcatname,decimal minPrice, decimal maxPrice)
    {
        try
        {
            catname = catname.Replace('-', ' ');

            // Fetch category
            Category = await _context.TblCategory.SingleOrDefaultAsync(e => e.MenuList == catname);

            if (Category == null)
            {
                ModelState.AddModelError(string.Empty, "Category not found");
                return new BadRequestResult();
            }

            // Clean up subcategory name by removing unwanted parts
            if (subcatname.Contains('-'))
            {
                subcatname = subcatname.Replace("-", " ").Replace("Crystals", "");
            }

            // Fetch subcategory
            Subcategory = await _context.TblSubcategory.SingleOrDefaultAsync(e => e.SubCategoryname == subcatname.TrimEnd().TrimStart());

            if (Subcategory == null)
            {
                ModelState.AddModelError(string.Empty, "Subcategory not found");
                return new BadRequestResult();
            }

            // Fetch subcategory list (though not clear why based on subcategory id, keeping it as in original code)
            SubCategoryList = await _context.TblSubcategory.Where(e => e.SubCategoryid == Subcategory.SubCategoryid).ToListAsync();

            // Prepare SQL parameters
            var parameters = new[]
            {
            new SqlParameter("@CategoryId", Category.Id),
            new SqlParameter("@SubCategory", Subcategory.SubCategoryid)
        };
            SPSubCategoryWiseProduct = await _context.SPSubCategoryWiseProduct
                  .FromSqlRaw("SPSubCategoryWiseProduct @CategoryId, @SubCategory", parameters)
                 .ToListAsync();
            var products = SPSubCategoryWiseProduct
                .AsEnumerable() // Switch to client evaluation
                .Where(p =>
                {
                    // Handle price ranges
                    if (p.Price.Contains('-'))
                    {
                        var priceParts = p.Price.Split('-');
                        if (priceParts.Length == 2 &&
                            decimal.TryParse(priceParts[0], out var rangeMin) &&
                            decimal.TryParse(priceParts[1], out var rangeMax))
                        {
                            return rangeMin <= maxPrice && rangeMax >= minPrice;
                        }
                    }

                    // Handle single prices
                    if (decimal.TryParse(p.Price, out var singlePrice))
                    {
                        return singlePrice >= minPrice && singlePrice <= maxPrice;
                    }

                    // Exclude invalid prices
                    return false;
                })
                .OrderBy(p =>
                {
                    // Sort by the lowest price in the range or the single price
                    if (p.Price.Contains('-'))
                    {
                        var priceParts = p.Price.Split('-');
                        if (decimal.TryParse(priceParts[0], out var lowestPrice))
                        {
                            return lowestPrice;
                        }
                    }

                    if (decimal.TryParse(p.Price, out var singlePrice))
                    {
                        return singlePrice;
                    }

                    return decimal.MaxValue; // Place invalid prices at the end
                })
                .Select(p => new
                {
                    p.SkuCode,
                    p.ProductName,
                    Price = p.Price,
                    p.Thumbnail
                })
                .ToList();

            return Ok(products);
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error in GetProductsByPriceRange: {ex.Message}");

            // Return a 500 response
            return StatusCode(500, new { error = "An error occurred while fetching products.", details = ex.Message });
        }
    }
}