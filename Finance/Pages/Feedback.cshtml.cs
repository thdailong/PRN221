using Finance.DAM;
using Finance.Model;
using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Finance.Pages
{
    [Authorize]
    public class FeedbackModel : PageModel
    {
        public Account Account { get => Identity.Get(HttpContext); }

        [BindProperty]
        [Required, StringLength(50, MinimumLength = 5)]
        public string Title { get; set; } = "";

        [BindProperty]
        [Required, StringLength(100, MinimumLength = 5)]
        public string Detail { get; set; } = "";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                await FeedbackDAM.Insert(Account.Email, Title, Detail);
                TempData["feedbackSent"] = true;
            }
            return Page();
        }
    }
}
