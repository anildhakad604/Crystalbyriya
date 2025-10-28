using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.Wishlist
{
    public class EditModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public EditModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AddingWishlist AddingWishlist { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addingwishlist =  await _context.TblWishlist.FirstOrDefaultAsync(m => m.Id == id);
            if (addingwishlist == null)
            {
                return NotFound();
            }
            AddingWishlist = addingwishlist;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(AddingWishlist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddingWishlistExists(AddingWishlist.Id))
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

        private bool AddingWishlistExists(int id)
        {
            return _context.TblWishlist.Any(e => e.Id == id);
        }
    }
}
