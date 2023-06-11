using Fatima.DAM;
using Fatima.Model;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;
using Transaction = Fatima.Model.Transaction;

namespace Fatima.Pages._Component.Home.History
{
    public class FormModel : _CompModel
    {
        public List<Category> ListCate { get; set; } = new();
        DateTime n = DateTime.Now;
        public string LocalDate;
        public string? Delete { get; set; } = "";
        public Transaction? Transaction { get; set; } = new();
        public class FormRecord
        {
            [BindProperty]
            public int Id { get; set; }
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
            var transactionId = json.GetProperty("transactionId").GetInt32();
            var email = json.GetProperty("email").GetString();
            var cateType = json.GetProperty("cateType").GetString();
            try
            {
                Delete = json.GetProperty("delete").GetString();
            }
            catch (KeyNotFoundException)
            {
                Delete = "";
            }
            ListCate = await CategoryDAM.ListCategoryByType(email, cateType);
            Transaction = await TransactionDAM.GetTransaction(email, transactionId);
            if (Transaction != null)
            {
                f.Id = Transaction.Id;
                f.CateId = Transaction.Category.Id.ToString();
                f.Date = Transaction.Date;
                f.Desc = Transaction.Description;
                f.Amount = Transaction.Amount;
                string month = Transaction.Date.Month >= 10 ? Transaction.Date.Month + "" : "0" + Transaction.Date.Month;
                string day = Transaction.Date.Day >= 10 ? Transaction.Date.Day + "" : "0" + Transaction.Date.Day;
                string hour = Transaction.Date.Hour >= 10 ? Transaction.Date.Hour + "" : "0" + Transaction.Date.Hour;
                string minute = Transaction.Date.Minute >= 10 ? Transaction.Date.Minute + "" : "0" + Transaction.Date.Minute;
                LocalDate = $"{Transaction.Date.Year}-{month}-{day}T{hour}:{minute}";
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDelete([FromQuery] int id, [FromQuery] string type)
        {
            await TransactionDAM.DeleteTransaction(id);
            string url = type == "income" ? "~/history?IncomeCheck=true" : "~/history";
            return Redirect(url);
        }

        public async Task<IActionResult> OnPostUpdate([FromQuery] int id, [FromQuery] string type)
        {
            await TransactionDAM.UpdateTransaction(id, f.Date, f.CateId, f.Desc, f.Amount);
            string url = type == "income" ? "~/history?IncomeCheck=true" : "~/history";
            return Redirect(url);
        }
    }
}
