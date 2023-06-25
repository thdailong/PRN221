using Finance.Model;
using Finance.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.Text.Json;

namespace Finance.Pages._Component.Home
{
    public class Test1Model : _CompModel
    {

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost([FromBody] JsonElement json)
        {
            var account = json.Deserialize<Account>(Helper.JsonOptions);
            Log.Debug("Account = {@Account}", account);
            return Page();
        }

        public class XYZ
        {
            public string abc { get; set; }
        }

        public IActionResult OnPostTest1([FromForm] XYZ xyz)
        {
            Log.Debug("{@XYZ}", xyz);
            return Page();
        }
    }
}
