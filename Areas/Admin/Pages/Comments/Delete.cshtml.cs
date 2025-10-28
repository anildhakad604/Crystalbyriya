using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Areas.Admin.Pages.Comments
{
    public class DeleteModel : PageModel
    {
        private readonly CrystalByRiya.Models.ApplicationDbContext _context;

        public DeleteModel(CrystalByRiya.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CommentReply CommentReply { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commentreply = await _context.TblCommentReply.FirstOrDefaultAsync(m => m.Id == id);

            if (commentreply is not null)
            {
                CommentReply = commentreply;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commentreply = await _context.TblCommentReply.FindAsync(id);
            if (commentreply != null)
            {
                CommentReply = commentreply;
                _context.TblCommentReply.Remove(CommentReply);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
