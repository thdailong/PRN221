using Dapper;
using Finance.Model;
using Finance.Utils;

namespace Finance.DAM
{
    public static class StatsDAM
    {
        public static async Task<StatsData> GetStatsFilter(string Email, string FromDate, string ToDate, string Freq, List<string> CateId)
        {
            string cateFiter = "";
            Console.WriteLine(Email + FromDate + ToDate + Freq);
            if (CateId.Count > 0)
            {
                cateFiter = "and cates.Id in (" + string.Join(", ", CateId) + ")";
            }
            var sql = @$"with tx as (
				select 
					*,
					Date,
					strftime('%Y-%m-%d', date) as ymd,
					strftime('%Y', date) as y,
					strftime('%Y-%m', date) as ym,
					strftime('%Y-%W', date) as yw,
					case 
						when strftime('%w', date) ='0' then 'Sunday'
						when strftime('%w', date) ='1' then 'Monday'
						when strftime('%w', date) ='2' then 'Tuesday'
						when strftime('%w', date) ='3' then 'Wednesday'
						when strftime('%w', date) ='4' then 'Thursday'
						when strftime('%w', date) ='5' then 'Friday'
						else 'Saturday'
					end as dow

				from `Transaction` trans
				left join Category cates on trans.CategoryId = cates.Id
				left join Icon icon on cates.IconId = icon.Id
			 	where Email=@Email and date(Date) between @FromDate and @ToDate
			 	{cateFiter}
			),
			stats_spend as (
				select 
					sum(Amount) as spendingSum, 
					avg(Amount) as spendingAvg, 
					max(Amount) as spendingMax,  
					min(Amount) as spendingMin
				from tx where Type='spending'
			),
			stats_income as (
				select 
					sum(Amount) as incomeSum, 
					avg(Amount) as incomeAvg, 
					max(Amount) as incomeMax,  
					min(Amount) as incomeMin
				from tx where Type='income'
			),
			bar as (
				select
					json_group_array(json_object('label', label, 'spend', spend, 'income', income)) as bar
				from (	
					select
						{Freq} as label,
						sum(iif(Type='spending', Amount, 0)) as spend,
						sum(iif(Type='income', Amount, 0)) as income
					from tx
					group by {Freq}
				)
			),
			pie as (
				select
					json_group_array(json_object('label', label, 'type', type, 'spend', value)) as pie
				from (	
					select
						Name as label,
						Type as type,
						sum(Amount) as value
					from tx
					group by Name, Type
				)
			),
			top5cate_spend as (
				select 
					json_group_array(json_object('ClassName', ClassName, 'IconColorClass', IconColorClass, 'name', Name, 'amount', amount, 'cnt', cnt, 'ratio', ratio)) as cateSpending
				from (
					select *, round(amount/sum(amount) over()*100, 2) as ratio 
					from (
						select
							ClassName,
							IconColorClass,
							Name, 
							sum(Amount) as amount,
							count(*) as cnt
						from tx
						where Type = 'spending'
						group by ClassName, IconColorClass, Name
						order by Amount desc
						limit 5
					)
				)
			),
			top5cate_income as (
				select 
					json_group_array(json_object('ClassName', ClassName, 'IconColorClass', IconColorClass, 'name', Name, 'amount', amount, 'cnt', cnt, 'ratio', ratio)) as cateIncome
				from (
					select *, round(amount/sum(amount) over()*100, 2) as ratio 
					from (
						select 
							ClassName,
							IconColorClass,
							Name, 
							sum(Amount) as amount,
							count(*) as cnt
						from tx
						where Type = 'income'
						group by ClassName, IconColorClass, Name
						order by Amount desc
						limit 5
					)
				)
			),
			top5tran_spend as (
				select 
					json_group_array(json_object('ClassName', ClassName, 'IconColorClass', IconColorClass, 'name', Name, 'amount', amount, 'date', Date)) as tranSpending
				from (
					select
						ClassName,
						IconColorClass,
						Name, 
						Amount as amount,
						Date
					from tx
					where Type = 'spending'
					order by Amount desc
					limit 5
				)
			),
			top5tran_income as (
				select 
					json_group_array(json_object('ClassName', ClassName, 'IconColorClass', IconColorClass, 'name', Name, 'amount', amount, 'date', Date)) as tranIncome
				from (
					select
						ClassName,
						IconColorClass,
						Name, 
						Amount as amount,
						Date
					from tx
					where Type = 'income'
					order by Amount desc
					limit 5
				)
			)
			select * from stats_spend
			cross join stats_income
			cross join bar 
			cross join pie
			cross join top5cate_spend
			cross join top5cate_income
			cross join top5tran_spend
			cross join top5tran_income";
            Console.WriteLine(sql);
            return await Clients.Sql.QuerySingleAsync<StatsData>(sql, new
            {
                Email,
                FromDate,
                ToDate
            });
        }
    }
}
