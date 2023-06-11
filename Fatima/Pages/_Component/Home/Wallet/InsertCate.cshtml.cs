using Fatima.DAM;
using Fatima.Model;
using Fatima.Services;
using Fatima.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.Text.Json;

namespace Fatima.Pages._Component.Home
{
    public class InsertCateModel : _CompModel
    {

        public Account Account { get => Identity.Get(HttpContext); }

        public IActionResult OnGet()
        {
            return Page();
        }
        public List<Category> ListCate { get; set; } = new();


        public async Task<IActionResult> OnPostInsert([FromBody] JsonElement json)
        {
            var cateName = json.GetProperty("cateName").GetString();
            var cateType = json.GetProperty("cateType").GetString();
            await CategoryDAM.Insert(cateName, cateType, 1, Account.Email);
            ListCate = await CategoryDAM.ListCategoryByType(Account.Email, cateType);

            return Page();
        }

        public async Task<IActionResult> OnPostUpdate([FromBody] JsonElement json)
        {
            // id, name, color, className: class_name
            // iconId, iconClassName
            var id = json.GetProperty("id").GetInt32();
            var cateName = json.GetProperty("name").GetString();
            var cateType = json.GetProperty("cateType").GetString();
            var cateColor = json.GetProperty("color").GetString();
            var iconClassName = json.GetProperty("className").GetString();
            var iconId = json.GetProperty("iconId").GetInt32();


            await CategoryDAM.Update(id, cateName, iconId, cateColor);
            ListCate = await CategoryDAM.ListCategoryByType(Account.Email, cateType);

            return Page();
        }

        public async Task<IActionResult> OnPostDelete([FromBody] JsonElement json)
        {
            var id = json.GetProperty("id").GetInt32();
            var cateType = json.GetProperty("cateType").GetString();

            await CategoryDAM.Delete(id);
            ListCate = await CategoryDAM.ListCategoryByType(Account.Email, cateType);

            return Page();
        }
    }
}
