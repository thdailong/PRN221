using Dapper;
using Fatima.Model;
using Fatima.Utils;

namespace Fatima.DAM
{
    public static class WalletDAM
    {
        public static async Task<bool> Insert(DateTime Date, string CategoryId, string Description, Double Amount)
        {
            var query = "insert into 'Transaction'(Date, CategoryId, Description, Amount) values(@Date, @CategoryId, @Description, @Amount)";
            try
            {
                await Clients.Sql.ExecuteAsync(query, new
                {
                    Date,
                    CategoryId,
                    Description,
                    Amount,
                });
                return true;
            }
            catch
            {
                return false;
            }
        }
    
        public static async Task<List<WalletCateAmount>> GetListCateAmount(string Email, String Type)
        {
            string sql = @"
                select
                    strftime('%Y-%m', Date) as Ym,
	                c.Name,
	                c.IconColorClass,
	                i.ClassName,
	                round(sum(Amount), 2) as Amount,
	                count(*) as Cnt,
	                round(sum(Amount) / sum(sum(Amount)) over(partition by strftime('%Y-%m', Date)) * 100, 2) as MonthlyRatio,
	                round(sum(sum(Amount)) over(partition by strftime('%Y-%m', Date)), 2) as MonthlyAmount
                from `Transaction` as tx
                left join Category as c on tx.CategoryId = c.Id
                left join Icon as i on c.IconId = i.Id
                where Email = @Email
                and Type = @Type
                group by Ym, CategoryId
                having Ym >= '2022-01'
                order by Ym desc, Amount desc";
            return (await Clients.Sql.QueryAsync<WalletCateAmount>(sql, new
            {
                Email, Type
            })).AsList();
        } 
    }
}
