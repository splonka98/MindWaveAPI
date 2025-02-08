using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MindWaveApi.Pages
{
    public class UserModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public UserModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
