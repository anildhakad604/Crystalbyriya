using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
namespace CrystalByRiya.Areas.Admin.Pages.Announcements
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Announcement Announcement { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Announcement = await _context.TblAnnouncement.FindAsync(id);

            if (Announcement == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var announcement = await _context.TblAnnouncement.FindAsync(id);

            if (announcement == null)
            {
                return NotFound();
            }

            _context.TblAnnouncement.Remove(announcement);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}