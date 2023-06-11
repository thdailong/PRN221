using Dapper;
using Fatima.Model;
using Fatima.Utils;

namespace Fatima.DAM
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
