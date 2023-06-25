using Finance.DAM;
using Finance.Model;
using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Finance.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public StatsData Stats { get; set; } = new();
        /// <summary>
        /// get Account infos
        /// </summary>
        public Account Account { get => Identity.Get(HttpContext); }

        [BindProperty]

        /// <summary>
        /// amount of income
        /// </summary>
        public double IncomeAmount { get; set; }

        /// <summary>
        /// amount of spending
        /// </summary>
        public double SpendingAmount { get; set; }

        /// <summary>
        /// current balance
        /// </summary>
        public double TotalBalance { get; set; }

        /// <summary>
        /// amount of income today
        /// </summary>
        public double TodayIncome { get; set; }

        /// <summary>
        /// amount of spending today
        /// </summary>
        public double TodaySpending { get; set; }

        /// <summary>
        /// list of category name
        /// </summary>
        public List<string> lstCatName = new();

        /// <summary>
        /// list of category count
        /// </summary>
        public List<int> lstCatCount = new();

        /// <summary>
        /// list of most use category amount 
        /// </summary>
        public List<double> lstCatAmount = new();

        /// <summary>
        /// list of most use category icon class name 
        /// </summary>
        public List<string> lstIconClassName = new();

        /// <summary>
        /// List of all transaction
        /// </summary>
        public List<Transaction> LstTransactions = new();

        public List<Transaction> LstTopSpending = new();

        public List<Transaction> LstTopIncome = new();
        public bool IncomeCheck { get; set; } = false;

        /// <summary>
        /// list of most use category progress percent
        /// </summary>
        public int[] ProgressPercent { get; set; } = { 0, 0, 0, 0, 0 };

        public List<string> ProgressDay = new();

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger) => _logger = logger;

        /// <summary>
        /// Init
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            bool.TryParse(Request.Query["IncomeCheck"].FirstOrDefault("false"), out bool s);
            IncomeCheck = s;
            string strEmail = Account.Email;
            Stats = await StatsDAM.GetStatsFilter(
                Account.Email,
                getFmtDate(DateTime.Now.Date.AddDays(-30)),
                getFmtDate(DateTime.Now.Date),
                "ymd",
                new List<string>());
            LstTransactions = await DAM.TransactionDAM.ListTransaction(strEmail);
            List<Transaction> lstTop5Trans = await DAM.CategoryDAM.ListTop5Transaction(strEmail);
            List<Transaction> lstIncomeTransaction = await DAM.TransactionDAM.ListTransactionByType(strEmail, "income");
            List<Transaction> lstSpendingTransaction = await DAM.TransactionDAM.ListTransactionByType(strEmail, "spending");
            IncomeAmount = lstIncomeTransaction.Sum(x => x.Amount);
            SpendingAmount = lstSpendingTransaction.Sum(x => x.Amount);
            TotalBalance = IncomeAmount - SpendingAmount;
            TodayIncome = lstIncomeTransaction.Where(x =>
            x.Date.Date == DateTime.Now.Date).Sum(x => x.Amount);
            TodaySpending = lstSpendingTransaction.Where(x =>
            x.Date.Date == DateTime.Now.Date).Sum(x => x.Amount);

            lstTop5Trans = !IncomeCheck
                ? lstTop5Trans.Where(x => x.Category.Type == "spending").Take(5).ToList()
                : lstTop5Trans.Where(x => x.Category.Type == "income").Take(5).ToList();

            int count = 0;
            lstTop5Trans.ForEach(x =>
            {
                lstIconClassName.Add(x.Category.Icon.ClassName);
                lstCatCount.Add(x.TransCount);
                lstCatName.Add(x.Category.Name);
                lstCatAmount.Add(x.SumAmount);
                if (x.Category.Type == "income")
                {
                    ProgressPercent[count] = (int)(x.SumAmount * 100 / IncomeAmount);
                }
                if (x.Category.Type == "spending")
                {
                    ProgressPercent[count] = (int)(x.SumAmount * 100 / SpendingAmount);
                }
                count++;
            });
        }

        /// <summary>
        /// VND currency process
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public string GetVNDCurrency(double num)
        {
            if (num < 0)
            {
                num = -num;
                num = MaxThreeSignificantDigits(num);

                return "-" + (num >= 100000000
                    ? (num / 1000000D).ToString("0.#M")
                    : num >= 1000000
                    ? (num / 1000000D).ToString("0.##M")
                    : num >= 100000
                    ? (num / 1000D).ToString("0k")
                    : num >= 100000 ? (num / 1000D).ToString("0.#k") : num >= 1000 ? (num / 1000D).ToString("0.##k") : num.ToString("#,0"));
            }
            else
            {
                num = MaxThreeSignificantDigits(num);

                return num >= 100000000
                    ? (num / 1000000D).ToString("0.#M")
                    : num >= 1000000
                    ? (num / 1000000D).ToString("0.##M")
                    : num >= 100000
                    ? (num / 1000D).ToString("0k")
                    : num >= 100000 ? (num / 1000D).ToString("0.#k") : num >= 1000 ? (num / 1000D).ToString("0.##k") : num.ToString("#,0");
            }

        }

        /// <summary>
        /// Get max three significant digits
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        internal static double MaxThreeSignificantDigits(double x)
        {
            int i = (int)Math.Log10(x);
            i = Math.Max(0, i - 2);
            i = (int)Math.Pow(10, i);
            return x / i * i;
        }

        string getFmtDate(DateTime d)
        {
            string m = "" + d.Month;
            if (d.Month < 10)
            {
                m = "0" + m;
            }

            string day = "" + d.Day;
            if (d.Day < 10)
            {
                day = "0" + day;
            }

            return $"{d.Year}-{m}-{day}";
        }

    }
}