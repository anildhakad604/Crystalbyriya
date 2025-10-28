using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CrystalByRiya.Models; // Include your DbContext namespace

namespace CrystalByRiya.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckRegisterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CheckRegisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("CheckIfExists")]
        public async Task<IActionResult> CheckIfExists([FromForm] string email, [FromForm] string phone)
        {
            var emailExists = await _context.TblRegisters.AnyAsync(r => r.Email == email);
            var phoneExists = await _context.TblRegisters.AnyAsync(r => r.PhoneNumber == phone);

            return Ok(new
            {
                EmailExists = emailExists,
                PhoneExists = phoneExists
            });
        }
    }
}
