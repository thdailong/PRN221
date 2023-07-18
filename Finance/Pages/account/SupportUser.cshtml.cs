using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finance.Model;
using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Finance.Pages.account
{

    [Authorize]
    public class SupportUser : PageModel
    {
        public Account Account { get => Identity.Get(HttpContext); }
        public void OnGet()
        {
        }
    }
}