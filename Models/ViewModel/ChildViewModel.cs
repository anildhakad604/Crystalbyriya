using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models.ViewModel
{
    public class ChildViewModel
    {
        ApplicationDbContext _context;
        public List<TblCategory> Categories { get; set; }
        public List<Subcategory> SubCategories { get; set; }
        public ChildViewModel(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
