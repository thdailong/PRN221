using Fatima.DAM;
using Fatima.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Fatima.Pages._Component.Home.Wallet
{
    public class FormModel : _CompModel
    {
        public List<Category> ListCate { get; set; } = new();
        DateTime n = DateTime.Now;
        public string LocalDate;
        public class FormRecord
        {
            [BindProperty]
            public DateTime Date { get; set; }
            [BindProperty]
            public String CateId { get; set; }
            [BindProperty]
            public String Desc { get; set; }
            [BindProperty]
            public Double Amount { get; set; }

        }

        [BindProperty]
        public FormRecord f { get; set; } = new FormRecord();

        public async Task<IActionResult> OnPost([FromBody] JsonElement json)

        {
            LocalDate = $"{n.Year}-{n.Month}-{n.Day}T{n.Hour}:{n.Minute}";

            var email = json.GetProperty("email").GetString();
            var cateType = json.GetProperty("cateType").GetString();

            ListCate = await CategoryDAM.ListCategoryByType(email, cateType);
            return Page();
        }
    }
}
