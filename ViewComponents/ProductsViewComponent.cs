using CrystalByRiya.Models;
using CrystalByRiya.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Viraj.ViewComponents
{
    [ViewComponent(Name = "Category")]
    public class ProductsViewComponent:ViewComponent
    {
        ApplicationDbContext _context;
        public ProductsViewComponent(ApplicationDbContext context)
        {
                _context = context;
        }
        public List<TblCategory> Categories { get; set; }
        public List<Subcategory> SubCategories { get; set; }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Categories = await _context.TblCategory.ToListAsync();
            SubCategories = await _context.TblSubcategory.OrderBy(sub => sub.SubCategoryname) // Sort SubCategories alphabetically by Name
                .ToListAsync();
            ChildViewModel model = new ChildViewModel(_context)
            {
                Categories = Categories,
                SubCategories = SubCategories
            };

            return View("Category", model);
        }
    }
}
