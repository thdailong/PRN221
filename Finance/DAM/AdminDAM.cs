using Dapper;
using Finance.Model;
using Finance.Utils;

namespace Finance.DAM
{
    public static class AdminDAM
    {
        public static async Task<BottomMetric> GetBottomMetric()
        {
            var query = @"
                select CntUser, CntCate, CntTran, CntFeed From 
                (select count(*) as CntUser from `Account`)
                cross join (select count(*) as CntCate from `Category`)
                cross join (select count(*) as CntTran from `Transaction`)
                cross join (select count(*) as CntFeed from `Feedback`)";
            return (await Clients.Sql.QuerySingleAsync<BottomMetric>(query));
        }
        public static async Task<List<String>> GetBottomFeedback()
        {
            var query = @"
                select Title from `Feedback`
                order by CreatedAt desc
                limit 3";
            return (await Clients.Sql.QueryAsync<String>(query)).AsList();
        }
    }
}
