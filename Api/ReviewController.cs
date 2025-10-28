using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrystalByRiya.Api
{
    public class ReviewRequest
    {
string name {  get; set; }
        string review { get; set; }
        string ProductId { get; set; }
        int rating { get; set; }
        string Emailid { get; set; }
        string Name { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> OnPostReview(ReviewRequest request)
        {
            return Ok();
        }
    }
}
