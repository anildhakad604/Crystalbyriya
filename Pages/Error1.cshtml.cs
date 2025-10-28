using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace CrystalByRiya.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class Error1Model : PageModel
    {
        public string Currenturl { get; private set; }
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<Error1Model> _logger;

        public Error1Model(ILogger<Error1Model> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            Currenturl = HttpContext.Request.GetDisplayUrl();
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
