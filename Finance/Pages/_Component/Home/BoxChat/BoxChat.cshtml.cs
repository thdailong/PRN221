using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Finance.DAM;
using Finance.Model;
using Finance.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Finance.Pages._Component.Home.BoxChat
{
    public class BoxChat : _CompModel
    {
        public List<Support> Supports { get; set; }
        public Account Account { get => Identity.Get(HttpContext); }
        public async Task OnGetAsync()
        {
            Supports = await SupportDAM.GetMessageFromUser(Account.Email);
        }

        public async Task<IActionResult> OnPostInsert([FromBody] JsonElement json)
        {
            var Content = json.GetProperty("Content").GetString();
            var Email = json.GetProperty("Email").GetString();
            var FromUser = json.GetProperty("FromUser").GetBoolean() ? "1" : "0";
            await SupportDAM.Insert(Email, Content, FromUser);
            Supports = await SupportDAM.GetMessageFromUser(Account.Email);
            return Page();
        }


    }
}