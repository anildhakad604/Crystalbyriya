using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CrystalByRiya
{
    public class NewsletterRequest
    {
        public string email { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class Newsletter : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Newsletter(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostNewsletter(NewsletterRequest request)
        {
            try {
                if (request == null || string.IsNullOrWhiteSpace(request.email))
                {
                    return BadRequest("Email is required.");
                }

                var existingEmail = await _context.TblNewsLetters
                                            .FirstOrDefaultAsync(e => e.NewsletterEmail == request.email);

                if (existingEmail != null)
                {
                    return BadRequest("This email is already subscribed.");
                }

                var newSubscription = new NewsLetter
                {
                    NewsletterEmail = request.email
                };

                await _context.TblNewsLetters.AddAsync(newSubscription);
                await _context.SaveChangesAsync(); // Save the changes asynchronously

                return Ok("Subscription successful!");
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