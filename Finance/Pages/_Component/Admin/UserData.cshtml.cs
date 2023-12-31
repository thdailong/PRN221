using Finance.DAM;
using Finance.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Finance.Pages._Component.Admin
{
    [Authorize("AdminOnly")]
    public class UserDataModel : _CompModel
    {
        public List<Account> Accounts { get; set; }
        public async Task<IActionResult> OnPost()
        {
            Accounts = await AccountDAM.GetAll();

            return Page();
        }
    }
}
