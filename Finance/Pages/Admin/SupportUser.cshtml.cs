using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finance.DAM;
using Finance.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Finance.Pages.Admin
{
    [Authorize("AdminOnly")]
    public class SupportUser : PageModel
    {
        public List<String> ActiveUser { get; set; }
        public List<Support> ActiveSupport { get; set; }
        public Boolean queryEmail { get; set; }
        public String chattingEmail { get; set; }
        public async Task OnGetAsync()
        {
            var userEmail = Request.Query["userEmail"].FirstOrDefault("admin");
            ActiveUser = await SupportDAM.GetAllUserInbox();
            ActiveSupport = new List<Support>();
            foreach (var item in ActiveUser)
            {
                var listSupport = await SupportDAM.LastMessageFromUser(item);
                ActiveSupport.Add(listSupport[0]);
            }
            var _find = ActiveUser.Find(e => e == userEmail);
            if (userEmail == "admin" || ActiveUser.Find(e => e == userEmail) == null)
            {
                queryEmail = true;
                chattingEmail = "";
            }
            else
            {
                queryEmail = false;
                chattingEmail = userEmail;
            }
        }
    }
}