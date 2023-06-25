using Finance.DAM;
using Finance.Model;
using Finance.Services;
using Finance.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Finance.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        public class FormInfoModel
        {
            [BindProperty]
            [Required, StringLength(60, MinimumLength = 3, ErrorMessage = "Name length must be between 3 and 60")]
            public string DisplayName { get; set; } = "";

            [BindProperty]
            public IFormFile? AvatarUpload { get; set; }
        }
        public class FormPasswordModel
        {
            [BindProperty]
            [Required, StringLength(60, MinimumLength = 5, ErrorMessage = "Length must be between 5 and 60")]
            public string OldPassword { get; set; } = "";

            [BindProperty]
            [Required, StringLength(60, MinimumLength = 5, ErrorMessage = "Length must be between 5 and 60")]
            public string Password { get; set; } = "";

            [BindProperty]
            [Required, DisplayName("Confirm password"), Compare("Password")]
            public string RePassword { get; set; } = "";
        }

        public Account Account { get => Identity.Get(HttpContext); }

        [BindProperty]
        public FormPasswordModel FormPassword { get; set; } = new();

        [BindProperty]
        public FormInfoModel FormInfo { get; set; } = new();

        public ProfileModel() : base()
        {

        }

        public void OnGet()
        {
            FormInfo.DisplayName = Account.DisplayName;
        }

        public async Task<IActionResult> OnPostUpdateInfo()
        {
            ModelState.Clear();
            if (!TryValidateModel(FormInfo, nameof(FormInfo)))
            {
                TempData.Remove("profileUpdateSuccess");
                return Page();
            }

            if (FormInfo.AvatarUpload != null)
            {
                var resp = await Helper.UploadFile("/Finance", FormInfo.AvatarUpload);
                Account.AvatarUrl = resp.URL;
            }
            Account.DisplayName = FormInfo.DisplayName;
            await AccountDAM.UpdateInfo(Account);

            TempData["profileUpdateSuccess"] = "Updated information successfully";
            return Page();
        }

        public async Task<IActionResult> OnPostUpdatePassword()
        {
            FormInfo.DisplayName = Account.DisplayName;
            ModelState.Clear();
            if (!TryValidateModel(FormPassword, nameof(FormPassword)))
            {
                TempData.Remove("profileUpdateSuccess");
                ViewData["openPassForm"] = true;
                return Page();
            }

            if (!await AccountDAM.Verify(Account.Email, FormPassword.OldPassword))
            {
                ModelState.AddModelError("FormPassword.OldPassword", "Old password is incorrect");
                TempData.Remove("profileUpdateSuccess");
                ViewData["openPassForm"] = true;
                return Page();
            }

            await AccountDAM.UpdatePassword(Account.Email, FormPassword.Password);
            TempData["profileUpdateSuccess"] = "Password changed successfully";
            return Page();
        }
    }
}
