using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Areas.Admin.Pages.Announcements
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Announcement> Announcements { get; set; }

        public async Task OnGetAsync()
        {
            Announcements = await _context.TblAnnouncement.ToListAsync();
        }
    }
}
