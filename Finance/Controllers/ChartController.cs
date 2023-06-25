using Dapper;
using Finance.DAM;
using Finance.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Controllers
{

    public class ChartController : APIBase
    {
        class ChartData
        {
            public string Label { get; set; }
            public double Value { get; set; }
        }
        [HttpGet("wallet/pie/{Email}/{Type}")]
        public async Task<ActionResult<object>> GetCateChart(string Email, string Type)
        {
            string sql = @"
                select sum(Amount)/ sum(sum(Amount)) over() as Value, c.Name as Label from 'Transaction' as tx 
                left join Category as c on  
                tx.CategoryId = c.Id
                where c.Email=@Email and c.Type=@Type
                group by c.Name
                ";
            return (await Clients.Sql.QueryAsync<ChartData>(sql, new
            {
                Email,
                Type
            })).AsList();

        }

        class AdminChartData
        {
            public string Label { get; set; }
            public int TodayCnt { get; set; }
            public int YesterdayCnt { get; set; }
        }
        [HttpGet("admin/chart")]
        public async Task<ActionResult<object>> GetAdminChart()
        {
            string sql = @"
                with base as (
	                SELECT 0 as Label UNION select 2 UNION select 4 UNION select 6 UNION select 8 UNION select 10
	                UNION select 12 UNION select 14 UNION select 16 UNION select 18 UNION select 20 UNION select 22
                )
                select * From base
                left join (
	                select strftime('%H', CreatedAt)/2*2 as Label, count(*) as YesterdayCnt from `Transaction` where date(CreatedAt) = DATE(date ( 'now', 'localtime' ),'-1 day') group by Label
                ) using(Label)

                left join (
	                select strftime('%H', CreatedAt)/2*2 as Label, count(*) as TodayCnt from `Transaction` where date(CreatedAt) = date ( 'now', 'localtime' ) group by Label
                ) using(Label)";
            return (await Clients.Sql.QueryAsync<AdminChartData>(sql)).AsList();

        }
    }
}
