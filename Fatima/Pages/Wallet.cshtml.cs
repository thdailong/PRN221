using Fatima.DAM;
using Fatima.Model;
using Fatima.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.Globalization;

namespace Fatima.Pages
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class WalletModel : PageModel
    {
        public Account Account { get => Identity.Get(HttpContext); }

        public bool IncomeCheck { get; set; } = false;
        public List<Category> ListCate { get; set; } = new();
        public List<Icon> ListIcon { get; set; } = new();
        DateTime n = DateTime.Now;
        public string LocalDate;
        List<WalletCateAmount> ListCateAmount { get; set; } = new();

        public class CateByMonth
        {
            public string Ym { get; set; }
            public double MonthlyAmount { get; set; }
            public List<WalletCateAmount> ListAmount { get; set; } = new();

        }
        public List<CateByMonth> ListCateAmountByMonth { get; set; } = new();
        public double SumAmount { get; set; }
        public async Task OnGet()
        {
            LocalDate = $"{n.Year}-{n.Month}-{n.Day}T{n.Hour}:{n.Minute}";
            bool.TryParse(Request.Query["IncomeCheck"].FirstOrDefault("false"), out bool s);
            IncomeCheck = s;
            // list all cate
            ListCate = await CategoryDAM.ListCategoryByType(Account.Email, s ? "income" : "spending");
            ListIcon = await IconDAM.ListIcon();
            ListCateAmount = await WalletDAM.GetListCateAmount(Account.Email, s ? "income" : "spending");
            foreach (var i in ListCateAmount)
            {
                SumAmount += i.Amount;
                if (ListCateAmountByMonth.FindAll(e => e.Ym == i.Ym).Count == 0)
                {
                    ListCateAmountByMonth.Add(new CateByMonth
                    {
                        Ym = i.Ym,
                        MonthlyAmount = i.MonthlyAmount,
                        ListAmount = ListCateAmount.FindAll(e => e.Ym == i.Ym)

                    });
                }
            }
            Log.Debug("{@asasd}", ListCateAmountByMonth);
            // insert cate
            // update cate
            // delete cate
        }
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
        public async Task<IActionResult> OnPost()
        {
            await WalletDAM.Insert(f.Date, f.CateId, f.Desc, f.Amount);
            return Redirect("./wallet");
        }

        public string GetStringFormatAmount(double num)
        {
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.###M", CultureInfo.InvariantCulture);
            }
            else if (num > 999 || num < -999)
            {
                return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
