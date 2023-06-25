using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Finance.Pages._Component
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public abstract class _CompModel : PageModel
    {
    }
}
