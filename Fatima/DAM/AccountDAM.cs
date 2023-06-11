using Dapper;
using Fatima.Model;
using Fatima.Utils;
using System.Security.Cryptography.X509Certificates;

namespace Fatima.DAM
{
    public static class AccountDAM
    {
        private static string Hash(string txt)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(txt);
        }

        public static async Task<bool> Verify(string email, string raw)
        {
            var hashedPwd = await Clients.Sql.QuerySingleAsync<string>("select Password from Account where Email=@email", new { email });
            return BCrypt.Net.BCrypt.EnhancedVerify(raw, hashedPwd);
        }

        public static async Task<bool> SignUp(Account account, string password)
        {
            var hashed = Hash(password);
            var query = "insert into Account(Email, Password, DisplayName) values(@Email, @Password, @DisplayName)";
            try
            {
                await Clients.Sql.ExecuteAsync(query, new
                {
                    account.Email,
                    account.DisplayName,
                    Password = hashed
                });
                return true;
            } catch
            {
                return false;
            }
        }

        public static async Task<bool> SignIn(string email, string password)
        {
            try
            {
                var ok = await Verify(email, password);
                if (!ok) throw new Exception("Incorrect password");
                return true;
            } catch
            {
                return false;
            }
        }

        public static async Task<Account> Get(string email)
        {
            return await Clients.Sql.QuerySingleAsync<Account>("select * from Account where Email=@email", new { email });
        }

        public static async Task UpdateActive(string email)
        {
            await Clients.Sql.ExecuteAsync("update Account set LastActive=datetime('now', 'localtime') where Email=@email", new { email });
        }

        public static Task<DateTime> GetLastActive(string email)
        {
            return Clients.Sql.QuerySingleAsync<DateTime>("select LastActive where Email=@email", new { email });
        }

        public static async Task UpdateInfo(Account account)
        {
            await Clients.Sql.ExecuteAsync($"{Helper.SqlUpdate("Account", "DisplayName", "AvatarUrl")} where Email=@Email", account);
        }

        public static async Task UpdatePassword(string Email, string password)
        {
            var hashed = Hash(password);
            await Clients.Sql.ExecuteAsync($"{Helper.SqlUpdate("Account", "Password")} where Email=@Email", new { Email, Password=hashed });
        }

        public static async Task<List<Account>> GetAll()
        {
            return (await Clients.Sql.QueryAsync<Account>("select * from Account order by CreatedAt desc")).AsList();
        }
    }
}
