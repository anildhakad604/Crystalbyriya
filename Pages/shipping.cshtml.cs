using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrystalByRiya.Pages
{
    public class shippingModel : PageModel
    {
        public string Currenturl { get; private set; }

        public void OnGet()
        {
            Currenturl = HttpContext.Request.GetDisplayUrl();
        }
    }
}
