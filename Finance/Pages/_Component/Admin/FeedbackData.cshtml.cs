using Finance.DAM;
using Finance.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace Finance.Pages._Component.Admin
{
    public class FeedbackDataModel : _CompModel
    {
        public List<Feedback> Feedbacks { get; set; } = null!;
        public async Task<IActionResult> OnPost()
        {
            Feedbacks = await FeedbackDAM.GetAll();
            return Page();
        }
    }
}
