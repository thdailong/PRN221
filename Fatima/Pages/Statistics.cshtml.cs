using Fatima.DAM;
using Fatima.Model;
using Fatima.Services;
using Fatima.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.Text.Json;

namespace Fatima.Pages
{
    public class StatisticsModel : PageModel
    {
        public StatsData Stats { get; set; } = new();
        public string IncomeSum { get; set; }
        public string IncomeAvg { get; set; }
        public string IncomeMax { get; set; }
        public string IncomeMin { get; set; }
        public string SpendingSum { get; set; }
        public string SpendingAvg { get; set; }
        public string SpendingMax { get; set; }
        public string SpendingMin { get; set; }
        public class CateData
        {
            public string ClassName { get; set; }
            public string IconColorClass { get; set; }
            public string Name { get; set; }
            public double Amount { get; set; }
            public int Cnt { get; set; }
            public double Ratio { get; set; }
        }
        public List<CateData> SpendCateData { get; set; } = new();
        public List<CateData> IncomeCateData { get; set; } = new();
        public class TranData
        {
            public string ClassName { get; set; }
            public string IconColorClass { get; set; }
            public string Name { get; set; }
            public double Amount { get; set; }
            public string Date { get; set; }
        }
        public List<TranData> SpendTranData { get; set; } = new();
        public List<TranData> IncomeTranData { get; set; } = new();


        [BindProperty]
        public DateTime FromDate { get; set; }
        [BindProperty]
        public DateTime ToDate { get; set; }
        [BindProperty]
        public string Freq { get; set; } = "ymd";
        //[BindProperty]
        //public string[] CateSpendIds { get; set; } = new string[100];
            
        //[BindProperty]
        //public string[] CateIncomeIds { get; set; } = new string[100];



        public string LocalDate;
        public List<Category> ListIncomeCate { get; set; } = new();
        public List<Category> ListSpendCate { get; set; } = new();
        public Account Account { get => Identity.Get(HttpContext); }

        string getFmtDate(DateTime d)
        {
            string m = ""+d.Month;
            if (d.Month < 10) m = "0" + m;
            string day = ""+d.Day;
            if (d.Day < 10) day = "0" + day;
            return $"{d.Year}-{m}-{day}";
        }

        public async Task OnGet(List<string> ListId, DateTime FromDate, DateTime ToDate, string? Freq)
        {
            DateTime n = DateTime.Now;

            if (Freq != null)
            {
                this.FromDate = FromDate;
                this.ToDate = ToDate!;
                this.Freq = Freq;
            }
            else { 
                this.FromDate = n.Subtract(TimeSpan.FromDays(30));
                this.ToDate = n;
            }
            Log.Debug("{@asfad}", ListId);
            Log.Debug("{@asfad}", FromDate);
            
            Stats = await StatsDAM.GetStatsFilter(
                Account.Email,
                getFmtDate(this.FromDate), 
                getFmtDate(this.ToDate),
                this.Freq,
                ListId);

            IncomeSum = Helper.FormatNumber(Stats.IncomeSum);
            IncomeAvg = Helper.FormatNumber(Stats.IncomeAvg);
            IncomeMax = Helper.FormatNumber(Stats.IncomeMax);
            IncomeMin = Helper.FormatNumber(Stats.IncomeMin);
            SpendingSum = Helper.FormatNumber(Stats.SpendingSum);
            SpendingAvg = Helper.FormatNumber(Stats.SpendingAvg);
            SpendingMax = Helper.FormatNumber(Stats.SpendingMax);
            SpendingMin = Helper.FormatNumber(Stats.SpendingMin);
            SpendCateData = JsonSerializer.Deserialize<List<CateData>>(Stats.CateSpending, Helper.JsonOptions);
            IncomeCateData = JsonSerializer.Deserialize<List<CateData>>(Stats.CateIncome, Helper.JsonOptions);
            SpendTranData = JsonSerializer.Deserialize<List<TranData>>(Stats.TranSpending, Helper.JsonOptions);
            IncomeTranData = JsonSerializer.Deserialize<List<TranData>>(Stats.TranIncome, Helper.JsonOptions);

            ListIncomeCate = await CategoryDAM.ListCategoryByType(Account.Email, "income");
            ListSpendCate = await CategoryDAM.ListCategoryByType(Account.Email, "spending");
            Log.Debug("{@asd}", ListSpendCate.Count);

        }
    }
}
