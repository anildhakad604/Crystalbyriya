using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Areas.Admin.Pages.Announcements
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Announcement Announcement { get; set; }

        public async Task OnGetAsync(int id)
        {
            Announcement = await _context.TblAnnouncement.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
