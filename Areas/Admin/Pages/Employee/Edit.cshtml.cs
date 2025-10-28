using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CrystalByRiya.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Areas.Admin.Pages.Employee
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CrystalByRiya.Models.Employee EditEmployee { get; set; }

        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the employee by ID
            EditEmployee = await _context.TblEmployee.FindAsync(id);

            if (EditEmployee == null)
            {
                return NotFound();
            }

            // If Rights are stored as comma-separated values, split them into a list
           

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Find the employee in the database by ID
            var employeeToUpdate = await _context.TblEmployee.FindAsync(id);

            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            // Update all necessary properties of the employee
            employeeToUpdate.Username = EditEmployee.Username;
            employeeToUpdate.Email = EditEmployee.Email;
            employeeToUpdate.Password = EditEmployee.Password;
            employeeToUpdate.ContactNo = EditEmployee.ContactNo;
            employeeToUpdate.Role = EditEmployee.Role;
            // Join selected rights as a comma-separated string


            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employeeToUpdate.Id))
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

        private bool EmployeeExists(int id)
        {
            return _context.TblEmployee.Any(e => e.Id == id);
        }

    }
}
