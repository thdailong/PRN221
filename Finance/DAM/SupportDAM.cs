using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Finance.Model;
using Finance.Utils;

namespace Finance.DAM
{
    public class SupportDAM
    {
        public static async Task Insert(string Email, string Content, string FromUser)
        {
            await Clients.Sql.ExecuteAsync(Helper.SqlInsert("Support", "Email", "Content", "FromUser"), new
            {
                Email,
                Content,
                FromUser
            });
        }

        public static async Task<List<Support>> GetAll()
        {
            var query = @"
                select * from Support s
                left join Account a on s.Email=a.Email
                order by s.CreatedAt asc";
            return (await Clients.Sql.QueryAsync<Support, Account, Support>(
                query,
                (support, account) =>
                {
                    support.Account = account;
                    return support;
                },
                splitOn: "Email"
                )).AsList();
        }

        public static async Task<List<Support>> GetMessageFromUser(string email)
        {
            var query = @"
                select * from Support s
                left join Account a on s.Email=a.Email
                where s.Email=@email
                order by s.CreatedAt asc";
            return (await Clients.Sql.QueryAsync<Support, Account, Support>(
                query,
                (support, account) =>
                {
                    support.Account = account;
                    return support;
                },
                new
                {
                    email
                },
                splitOn: "Email"
                )).AsList();
        }

        public static async Task<List<String>> GetAllUserInbox()
        {
            var query = @"
                SELECT DISTINCT Email FROM Support c ";
            return (await Clients.Sql.QueryAsync<String>(query)).AsList();
        }

        public static async Task<List<Support>> LastMessageFromUser(string email)
        {
            var query = @"
                select * from Support s
                left join Account a on s.Email=a.Email
                where s.Email=@email
                order by s.CreatedAt desc
                LIMIT 1";
            return (await Clients.Sql.QueryAsync<Support, Account, Support>(
                query,
                (support, account) =>
                {
                    support.Account = account;
                    return support;
                },
                new
                {
                    email
                },
                splitOn: "Email"
                )).AsList();
        }

    }
}