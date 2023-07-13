using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Finance.Pages
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await Identity.Logout(HttpContext);
            return Redirect(Request.Query["ReturnUrl"].FirstOrDefault("/"));
        }
    }
}
