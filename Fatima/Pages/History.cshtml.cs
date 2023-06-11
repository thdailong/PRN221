using Fatima.DAM;
using Fatima.Model;
using Fatima.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System;
using System.Globalization;
using System.Text.Json;
using static Fatima.Pages.StatisticsModel;

namespace Fatima.Pages
{
    public class HistoryModel : PageModel
    {
        public Account Account { get => Identity.Get(HttpContext); }
        [BindProperty]
        public List<Category> Categories { get; set; } = new();
        public bool IncomeCheck { get; set; } = false;
        public List<Transaction> Transactions  { get; set; } = new();
        public List<Transaction> TransDate  { get; set; } = new();
        public DateTime Date { get; set; }
        [BindProperty]
        public DateTime DateFrom { get; set; } = DateTime.Now;
        [BindProperty]
        public DateTime DateTo { get; set; } = DateTime.Now;
        public DateTime DateMin { get; set; } = DateTime.Now;
        public DateTime DateMax { get; set; } = DateTime.Now;
        public bool IsFirstMonthRender { get; set; } = true;

        public double Total = 0;
        public double TotalAmount
        {
            get
            {
                foreach (var transaction in Transactions)
                {
                    Total += transaction.Amount;
                }
                return Total;
            }
            set { Total = value; }
        }

        public async Task OnGet(List<int> ListId, DateTime DateFrom, DateTime DateTo)
        {
            bool.TryParse(Request.Query["IncomeCheck"].FirstOrDefault("false"), out bool s);
            Log.Debug("s: {@asfad}",s);
            IncomeCheck = s;
            
            Total = 0;
            Log.Debug("List: {@asfad}", ListId.ToArray());
            Log.Debug("Date From: {@asfad}", DateFrom);
            Log.Debug("Date To: {@asfad}", DateTo);
            if (ListId.Count > 0 || (ListId.Count == 0 && DateFrom > new DateTime() && DateTo > new DateTime()))
            {
                if (DateFrom > DateTo)
                {
                    Transactions = new();
                } else
                {
                    if (DateFrom == DateTo)
                    {
                        DateTo = new DateTime(DateFrom.Year, DateFrom.Month, DateFrom.Day, 23, 59, 59);
                    }
                    if (ListId.Count == 0)
                    {
                        Transactions = await TransactionDAM.ListTransactionByDate(Account.Email, IncomeCheck ? "income" : "spending", DateFrom, DateTo);
                    }
                    else
                    {
                        Transactions = await TransactionDAM.ListTransactionByDateAndCate(Account.Email, IncomeCheck ? "income" : "spending", DateFrom, DateTo, ListId);
                    }
                }
                
                TransDate = await TransactionDAM.ListTransactionByType(Account.Email, IncomeCheck ? "income" : "spending");
                var check = TransDate.FirstOrDefault();
                if (check != null)
                {
                    Date = check.Date;
                    DateMax = check.Date;
                    var oldestDate = TransDate.LastOrDefault();
                    if (oldestDate != null)
                    {
                        DateMin = oldestDate.Date;
                    }
                };

                this.DateTo = DateTo;
                this.DateFrom = DateFrom;
            } else
            {
                Transactions = await TransactionDAM.ListTransactionByType(Account.Email, IncomeCheck ? "income" : "spending");
                var check = Transactions.FirstOrDefault();
                if (check != null)
                {
                    Date = check.Date;
                    this.DateTo = check.Date;
                    DateMax = check.Date;
                    var oldestDate = Transactions.LastOrDefault();
                    if (oldestDate != null)
                    {
                        this.DateFrom = oldestDate.Date;
                        DateMin = oldestDate.Date;
                    }
                };
            }

            
            await GetListCateByType();
        }

        /// <summary>
        /// Get list cate by type when user modify transaction
        /// </summary>
        /// <param name="type"></param>
        public async Task GetListCateByType()
        {
            Categories = await CategoryDAM.ListCategoryByType(Account.Email, IncomeCheck ? "income" : "spending");
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

        public string LocalDate(DateTime date)
        {
            string LocalDate;
            string month = date.Month >= 10 ? date.Month + "" : "0" + date.Month;
            string day = date.Day >= 10 ? date.Day + "" : "0" + date.Day;
            LocalDate = $"{date.Year}-{month}-{day}";
            return LocalDate;
        }

        public string TotalInMonth(Transaction tran)
        {
            double TotalInMonth = 0;
            DateTime date = tran.Date;
            foreach (var transaction in Transactions)
            {
                if (date.Year == transaction.Date.Year && date.Month == transaction.Date.Month)
                {
                    TotalInMonth += transaction.Amount;
                }
            }
            return GetStringFormatAmount(TotalInMonth);
        }
    }
}
