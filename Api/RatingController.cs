using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CrystalByRiya.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RatingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetRating(string productid)
        {
            try
            {
                var reviewsQuery = _context.TblReviews
                    .Where(r => r.Productid == productid)
                    .Select(r => r.Rating);

                var customerReviewCount = await _context.TblReviews.Where(rs => rs.Productid == productid).CountAsync();

                if (await reviewsQuery.AnyAsync())
                {
                    double averageRating = await reviewsQuery.AverageAsync();
                    return Ok(new { rating = Math.Round(averageRating), custom = customerReviewCount });
                }
                else
                {
                    return NotFound("No ratings found for this product.");
                }
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

    
