using Dapper;
using Fatima.Model;
using Fatima.Utils;

namespace Fatima.DAM
{
    public static class FeedbackDAM
    {
        public static async Task Insert(string Email, string Title, string Detail)
        {
            await Clients.Sql.ExecuteAsync(Helper.SqlInsert("Feedback", "Email", "Title", "Detail"), new
            {
                Email,
                Title,
                Detail
            });
        }

        public static async Task<List<Feedback>> GetAll()
        {
            var query = @"
                select * from Feedback f
                left join Account a on f.Email=a.Email
                order by f.CreatedAt desc";
            return (await Clients.Sql.QueryAsync<Feedback, Account, Feedback>(
                query,
                (f, a) => {
                    f.Account = a;
                    return f;
                },
                splitOn: "Email"
                )).AsList();
        }
    }
}
