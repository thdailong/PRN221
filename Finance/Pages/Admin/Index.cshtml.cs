using Finance.DAM;
using Finance.Model;
using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Finance.Pages.Admin
{
    [Authorize("AdminOnly")]
    public class IndexModel : PageModel
    {
        public List<String> Feedback { get; set; } = new();
        public BottomMetric Metric { get; set; } = new();
        public async Task OnGetAsync()
        {
            await AccountDAM.UpdateActive(Identity.Get(HttpContext).Email);
            Feedback = await AdminDAM.GetBottomFeedback();
            Metric = await AdminDAM.GetBottomMetric();
        }
    }
}
