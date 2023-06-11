namespace Fatima.Model
{
    public class StatsData
    {
        // top
        public double SpendingSum { get; set; }
        public double SpendingAvg { get; set; }
        public double SpendingMax { get; set; }
        public double SpendingMin { get; set; }
        public double IncomeSum { get; set; }
        public double IncomeAvg { get; set; }
        public double IncomeMax { get; set; }
        public double IncomeMin { get; set; }
        // barchart
        public string Bar{ get; set; }
        // pie chart of spending/income
        public string Pie { get; set; }
        // pie chart income
        // list top 5 cate spending
        // list top 5 cate income
        public string CateSpending { get; set; }
        public string CateIncome{ get; set; }
        // list top 5 trans spending
        // list top 5 trans income
        public string TranSpending { get; set; }
        public string TranIncome { get; set; }

    }
}
