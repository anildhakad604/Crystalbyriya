using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Areas.Admin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public int TblCustomerOrderDetail { get; set; }
        public double TotalSales { get; set; }
        [BindProperty]
        public int ProductSold { get; set; }
        public int TotalRegisteredUsers { get; set; }
        public int TotalProducts { get; set; }
        public double TotalOrderAmount { get; set; }

        public int TotalRegisteredUser { get; set; }
         public IList<Register> Registers { get; set; }
        public IList<TblOrderId> tblOrderIds { get; set; }

        public IList<Product> products { get; set; }
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnGet()
        {
            DateTime timeUtc = System.DateTime.UtcNow;
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
            string date = cstTime.ToString("dd-MM-yyyy");
            string time = cstTime.ToString("HH:mm");
            string logedin = HttpContext.Session.GetString("Login");

            //if (string.IsNullOrEmpty(logedin))
            //{
            //    return RedirectToPage("Login");
            //}
            //else
            //{


                //var empRecord = _context.SPTOTALSALES.FromSqlRaw("SPTOTALSALES").ToList();
                //int val = empRecord.FirstOrDefault().Totalsales;
                //ProductSold = val;

                //TotalSales = _context.TblOrderIds.Where(status=>status.Status=="Completed").Sum(id=>id.TotalAmount);
                //TotalRegisteredUser = _context.TblRegisters.Count();
                //TblCustomerOrderDetail = _context.TblCustomerOrderDetails.Where(status=>status.Status=="Completed").Sum(id => id.Qty);
                //TblOrderId =await _context.TblOrderIds.OrderByDescending(id => id.Id).ToListAsync();
                // Count total registered users
                TotalRegisteredUsers = await _context.TblRegisters.CountAsync();

                // Count total products
                TotalProducts = await _context.TblProducts.CountAsync();

                // Sum of TotalAmount for completed orders
                TotalOrderAmount = Math.Round(await _context.TblOrderIds
      .Where(order => order.PaymentStatus == "Completed")
      .SumAsync(order => order.TotalAmount), 2);
                
                return Page();
            

        }

      //  public IList<TblOrderId> TblOrderId { get; set; }
        public IActionResult OnGetLogout()
        {

            HttpContext.Session.Remove("Login");
            return Redirect("./Index");

        }

    }
}
