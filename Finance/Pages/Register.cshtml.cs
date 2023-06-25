using Finance.Model;
using Finance.Services;
using Finance.Utils;
using Imagekit.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.ComponentModel.DataAnnotations;
using static Finance.Utils.Helper;

namespace Finance.Pages
{
    [Authorize("GuestOnly")]
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public Account AccountInput { get; set; } = new();

        [BindProperty]
        [Required, StringLength(60, MinimumLength = 5, ErrorMessage = "Password length must be between 5 and 60")]
        public string Password { get; set; } = "";

        public QuoteData Quote = null!;

        public async Task OnGet()
        {
            Quote = await Helper.Quote.Get();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Quote = await Helper.Quote.Get();
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var res = await DAM.AccountDAM.SignUp(AccountInput, Password);
            if (!res)
            {
                ModelState.AddModelError("EmailExists", "Your email already existed");
                return Page();
            }

            TempData["registerSuccess"] = new Dictionary<string, string>
            {
                {"msg", "You successfully registered new account"},
                {"email", AccountInput.Email}
            };
            return Redirect("/login");
        }
    }
}
