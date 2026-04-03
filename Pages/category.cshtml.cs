using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CrystalByRiya.Pages
{
    public class categoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        // Constructor that accepts both the ApplicationDbContext and IConfiguration
        public categoryModel(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            // Get the connection string from appsettings.json using IConfiguration
            _connectionString = configuration.GetConnectionString("CrystalByRiyaConnection");
        }

        public string Currenturl { get; private set; }
        [BindProperty]
        public TblCategory Category { get; set; }
        [BindProperty]
        public List<Subcategory> SubCatList { get; set; }

        public Dictionary<string, int> SubcategoryProductCounts { get; set; } = new Dictionary<string, int>();

        public string Catname { get; set; }

        public async Task OnGet(string Url)
        {
            try
            {
                Catname = Url;
                Currenturl = HttpContext.Request.GetDisplayUrl();

                // Fetch category details based on the URL
                Category = await _context.TblCategory.SingleOrDefaultAsync(e => e.Url == Url);

                if (Category != null)
                {
                    // Fetch the subcategories for the given category
                    SubCatList = await _context.TblSubcategory.Where(e => e.CategoryId == Category.Id).ToListAsync();

                    // For each subcategory, call the stored procedure to get the product count
                    foreach (var subcategory in SubCatList)
                    {
                        int productCount = GetItemsByCategoryAndSubCategory(Category.Id, subcategory.SubCategoryid);
                        SubcategoryProductCounts[subcategory.SubCategoryname] = productCount;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        // Call the stored procedure to get the count of items for each subcategory
        private int GetItemsByCategoryAndSubCategory(int categoryId, int subcategoryId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CategoryId", categoryId);
                parameters.Add("@SubCategoryId", subcategoryId);

                // Execute the stored procedure and return the total product count
                return db.ExecuteScalar<int>("GetItemsByCategoryAndSubCategory", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}

