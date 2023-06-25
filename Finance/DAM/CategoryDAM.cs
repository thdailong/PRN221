using Dapper;
using Finance.Model;
using Finance.Utils;
using Microsoft.Extensions.Hosting;

namespace Finance.DAM
{
    public static class CategoryDAM
    {
        public static async Task<List<Category>> ListCategory(string email)
        {
            var sql = @"select * from Category c
                left join Account a on a.Email = c.Email
				left join Icon i on i.Id = c.IconId
                where c.Email=@email
				Order by c.Id";
            var data = await Clients.Sql.QueryAsync<Category, Account, Icon, Category>(sql, (category, account, icon) =>
            {
                category.Account = account;
                category.Icon = icon;
                return category;
            }, new { email }, splitOn: "Email, Id");
            return data.ToList();
        }

        public static async Task<List<Category>> ListCategoryByType(string email, string type)
        {
            var sql = @"select * from Category c
                left join Account a on a.Email = c.Email
				left join Icon i on i.Id = c.IconId
                where c.Email=@email and c.Type=@type
				Order by c.Id";
            var data = await Clients.Sql.QueryAsync<Category, Account, Icon, Category>(sql, (category, account, icon) =>
            {
                category.Account = account;
                category.Icon = icon;
                return category;
            }, new
            {
                email,
                type
            }, splitOn: "Email, Id");

            return data.ToList();
        }

        public static async Task<Category?> GetCategory(string email, string categoryId)
        {
            var sql = @"select * from Category c
                left join Account a on a.Email = c.Email
				left join Icon i on i.Id = c.IconId
                where c.Email=@email and c.Id =@categoryId";

            var data = await Clients.Sql.QueryAsync<Category, Account, Icon, Category>(sql, (category, account, icon) =>
            {
                category.Account = account;
                category.Icon = icon;
                return category;
            }, new { email, categoryId }, splitOn: "Email, Id");

            return data.FirstOrDefault();
        }
        public static async Task<bool> Insert(string Name, string Type, int IconId, string Email)
        {
            var query = "insert into Category(Name, Type, IconId, Email) values(@Name, @Type, @IconId, @Email)";
            try
            {
                await Clients.Sql.ExecuteAsync(query, new
                {
                    Name,
                    Type,
                    IconId,
                    Email,
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> Delete(int Id)
        {
            var query = "delete from Category where id=@Id";
            try
            {
                await Clients.Sql.ExecuteAsync(query, new
                {
                    Id
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<List<Transaction>> ListTop5Transaction(string email)
        {
            string sql = @"SELECT * FROM(
                            SELECT 
                              SUM(t.Amount) AS SumAmount,
                              t.CategoryId AS CategoryId, 
                              COUNT(*) AS TransCount
                            FROM 
                              ""Transaction"" t
                            GROUP BY t.CategoryId
                            ORDER BY Count(*) DESC) AS Top5
                            LEFT JOIN ""Category"" as Cat
                            ON Top5.CategoryId = Cat.Id
                            LEFT JOIN ""Icon"" AS ic
                            ON Cat.IconId = ic.Id
                            WHERE Email = @email";
            IEnumerable<Transaction> data = await Clients.Sql.QueryAsync<Transaction, Category, Icon, Transaction>(sql, (transaction, category, icon) =>
            {
                category.Icon = icon;
                transaction.Category = category;
                return transaction;
            }, new { email }, splitOn: "Id,Id");

            return data.OrderByDescending(x => x.SumAmount).ToList();
        }

        public static async Task Update(int Id, string Name, int IconId, string Color)
        {
            var query = "update Category set Name=@Name, IconId=@IconId, IconColorClass=@Color where Id=@Id";

            await Clients.Sql.ExecuteAsync(query, new
            {
                Name,
                IconId,
                Id,
                Color
            });


        }


    }
}
