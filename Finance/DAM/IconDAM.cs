using Dapper;
using Finance.Model;
using Finance.Utils;

namespace Finance.DAM
{
    public static class IconDAM
    {
        public static async Task<List<Icon>> ListIcon()
        {
            var sql = @"select * from Icon";
            return (await Clients.Sql.QueryAsync<Icon>(sql)).AsList();
        }
    }
}
