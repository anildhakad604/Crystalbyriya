using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Areas.Admin.Pages.Subcategories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Subcategory> Subcategories { get; set; }

        public async Task OnGetAsync()
        {
            Subcategories = await _context.TblSubcategory.ToListAsync();
        }
    }
}
