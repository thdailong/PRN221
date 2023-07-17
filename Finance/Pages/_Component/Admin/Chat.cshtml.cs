using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Finance.Pages._Component.Admin
{
    [Authorize]
    public class Chat : PageModel
    {

        public void OnGet()
        {
        }
    }
}