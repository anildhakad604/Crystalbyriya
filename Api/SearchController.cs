using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CrystalByRiya.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Search?name={name}
        [HttpGet]

        public IActionResult GetProducts(string name)
        {
            try
            {
                // Fetch only product names that match the search term (case-insensitive)
                var products = _context.TblProducts
                    .Where(q => q.ProductName.Contains(name)).ToList(); // Case-insensitive match



                // Return the product names as JSON
                return Ok(products);
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
