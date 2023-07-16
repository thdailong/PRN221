using Finance.Model;
using Finance.Services;
using Finance.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace Finance.Pages
{
    [Authorize("GuestOnly")]
    public class LoginModel : PageModel
    {
        [BindProperty, EmailAddress]
        public string Email { get; set; } = "";

        [BindProperty]
        [Required, StringLength(60, MinimumLength = 5, ErrorMessage = "Password length must be between 5 and 60")]
        public string Password { get; set; } = "";

        [BindProperty]
        public bool RememberMe { get; set; }

        public QuoteData Quote = null!;

        public async Task OnGet()
        {
            Quote = await Helper.Quote.Get();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Quote = await Helper.Quote.Get();
            Log.Debug("{@email}", Email);
            Log.Debug("{@pass}", Password);
            var res = await DAM.AccountDAM.SignIn(Email, Password);
            if (!res) ModelState.AddModelError("InputWrong", "Email or Password incorrect");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await Identity.Login(HttpContext, Email, RememberMe);
            return Redirect(Request.Query["ReturnUrl"].FirstOrDefault("/account"));
        }
    }
}
