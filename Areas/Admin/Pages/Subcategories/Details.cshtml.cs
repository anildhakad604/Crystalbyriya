using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CrystalByRiya.Areas.Admin.Pages.Subcategories
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Subcategory Subcategory { get; set; }

        public async Task OnGetAsync(int id)
        {
            Subcategory = await _context.TblSubcategory.FindAsync(id);
        }
    }
}
