/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public EditModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;
        public List<ProductSizes> ProductSizes { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product =  await _context.TblProducts.FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }
            Product = product;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return _context.TblProducts.Any(e => e.ID == id);
        }

    }
}
*//*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;
using System.Data;
using Newtonsoft.Json;
using System.Configuration;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CrystalByRiya.Areas.Admin.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;
        private readonly string _connectionstring;

        public EditModel(CrystalByRiya.Models.ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            _connectionstring = configuration.GetConnectionString("CrystalByRiyaConnection");
            
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        // Define ProductSizes to store sizes for the selected product
        public List<ProductSizes> ProductSizes { get; set; }

        // Load product and associated sizes on GET request
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the product
            Product = await _context.TblProducts.FirstOrDefaultAsync(m => m.ID == id);

            if (Product == null)
            {
                return NotFound();
            }

            // Fetch associated ProductSizes
            ProductSizes = await _context.TblProductSizes
     .Where(ps => ps.ProductID == Product.SkuCode)
     
     .ToListAsync();
             int SPCategoryandSubCategoryWiseProduct(string skucode)
            {
                using (IDbConnection db = new SqlConnection(_connectionstring))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@SkuCode",skucode);
                   

                    // Execute the stored procedure and return the total product count
                    return db.ExecuteScalar<int>("SPCategoryandSubCategoryWiseProduct", parameters, commandType: CommandType.StoredProcedure);
                }
            }



            return Page();
        }

        // Handle the POST request (save the changes)
        public async Task<IActionResult> OnPostAsync()
        {


            // Update the Product entity
            _context.Attach(Product).State = EntityState.Modified;

            // Retrieve ProductSizes from the form submission and update existing entries
               //foreach (var size in ProductSizes)
                //{
                //    // Check if the size exists in the database
                //    var existingSize = await _context.TblProductSizes
                //        .FirstOrDefaultAsync(s => s.SizeID == size.SizeID);

                //    if (existingSize != null)
                //    {
                //        // Update the existing size
                //        existingSize.Size = size.Size;
                //        existingSize.ImageURL = size.ImageURL;
                //        existingSize.StockQuantity = size.StockQuantity;
                //        existingSize.Price = size.Price;
                //        _context.Attach(existingSize).State = EntityState.Modified;
                //    }
                //}

                var existingsizecollection = await _context.TblProductSizes.Where(e => e.ProductID == Product.SkuCode).ToListAsync();
                if (existingsizecollection != null)
                {
                    _context.TblProductSizes.RemoveRange(existingsizecollection);
                    await _context.SaveChangesAsync();
                }

                string Saveasdraft = Request.Form["Sizehiddencollection"];
                DataTable dtSaveasdraft = JsonConvert.DeserializeObject<DataTable>(Saveasdraft);

                foreach (DataRow row in dtSaveasdraft.Rows)
                {
                    if (!string.IsNullOrEmpty(row[0].ToString()))
                    {
                        await _context.TblProductSizes.AddAsync(new ProductSizes
                        {
                            ProductID = Product.SkuCode,
                            ImageURL = row[2].ToString(),
                            Price = Convert.ToDouble(row[4]),
                            Size = row[1].ToString(),
                            SKUCode = row[0].ToString(),
                            StockQuantity = Convert.ToInt16(row[3])

                        });
                        await _context.SaveChangesAsync();
                    }
                }
            



            try
            {
                // Save the changes for both Product and ProductSizes
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        // Check if the product exists
        private bool ProductExists(int id)
        {
            return _context.TblProducts.Any(e => e.ID == id);
        }

    }
}
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using CrystalByRiya.Classes;
using System.Configuration;

namespace CrystalByRiya.Areas.Admin.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;
        private readonly string _connectionString;
       
     

        private readonly AmazonS3 _amazonS3;
        private readonly AwsCredentials awsCredentials;

        public EditModel(CrystalByRiya.Models.ApplicationDbContext context, AwsCredentials awsCredentials, AmazonS3 amazonS3, IConfiguration configuration)
        {
            _context = context;
            this.awsCredentials = awsCredentials;
            _connectionString = configuration.GetConnectionString("CrystalByRiyaConnection");
            _amazonS3 = amazonS3;
        }

        // BindProperty for Product entity
        [BindProperty]
        public Product Product { get; set; }

        public Material Material { get; set; }

        // BindProperty for storing associated ProductSizes
        [BindProperty]
        public List<ProductSizes> ProductSizes { get; set; }

        // BindProperty for storing the result of the stored procedure (Category and SubCategory information)
        [BindProperty]
        public List<Dictionary<string, string>> ProductDetailsList { get; set; } = new List<Dictionary<string, string>>();
        [BindProperty]
        public IList<Material> Materialnames { get; set; }
        public ProductSizes productSizes { get; set; }

        public string imgname { get; set; }
        // Handle the GET request to load the product data and associated sizes
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the product by ID
            Product = await _context.TblProducts.FirstOrDefaultAsync(m => m.ID == id);

            imgname = Product.Thumbnail;

            if (Product == null)
            {
                return NotFound();
            }

            // Fetch associated ProductSizes
            ProductSizes = await _context.TblProductSizes
                .Where(ps => ps.ProductID == Product.SkuCode)
                .ToListAsync();
            Materialnames = await _context.Materials
         .Where(m => m.SkuCode == Product.SkuCode)
         .ToListAsync();


            // Call the stored procedure to fetch Category and SubCategory data for the given SKU code
            ProductDetailsList = GetCategoryAndSubCategoryWiseProduct(Product.SkuCode);



            return Page();
        }

        // Handle the POST request to save the changes
        public async Task<IActionResult> OnPostAsync(IFormFile thumbnail, string thumbnailimg )
        {
            if (thumbnail != null && thumbnail.Length > 1024 * 1024) // 1 MB in bytes
            {
                ModelState.AddModelError("Product.Thumbnail", "The thumbnail size must not exceed 1 MB.");
                return Page(); // Return to the page with the validation error
            }
          
            if (thumbnail !=null)
            {
                string fileName = await _amazonS3.UploadFileToS3(thumbnail, awsCredentials.ProductsFoldername);
                Product.Thumbnail = fileName;
            }
            else
            {
                Product.Thumbnail = thumbnailimg;
            }

            // Attach and update the Product entity
            _context.Attach(Product).State = EntityState.Modified;



           

            try
            {
                // Save changes to both Product and ProductSizes
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        // Helper method to check if the product exists in the database

        private bool ProductExists(int id)
        {
            return _context.TblProducts.Any(e => e.ID == id);
        }

        // Method to call the stored procedure using Dapper and return the result as a list of dictionaries
        private List<Dictionary<string, string>> GetCategoryAndSubCategoryWiseProduct(string skuCode)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@SkuCode", skuCode);

                // Execute the stored procedure and fetch all rows
                var result = db.Query<dynamic>(
                    "SPCategoryandSubCategoryWiseProduct",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // Convert the result into a list of dictionaries, filtering out null SkuCode entries
                var productDetailsList = new List<Dictionary<string, string>>();

                foreach (var row in result)
                {
                    if (!string.IsNullOrEmpty(row?.SkuCode)) // Filter out rows where SkuCode is null
                    {
                        var productDetailsDict = new Dictionary<string, string>
                        {
                            { "CategoryId", row.Id.ToString() },
                            { "MenuList", row.MenuList ?? "N/A" },
                            { "SubCategoryId", row.SubCategoryId.ToString() },
                            { "SubCategoryName", row.SubCategoryName ?? "N/A" },
                            { "SkuCode", row.SkuCode }
                        };

                        productDetailsList.Add(productDetailsDict);
                    }
                }

                return productDetailsList;
            }
        }
    }
}

