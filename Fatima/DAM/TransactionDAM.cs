using Dapper;
using Fatima.Model;
using Fatima.Utils;

namespace Fatima.DAM
{
    public static class TransactionDAM
    {
        public static async Task<List<Transaction>> ListTransaction(string email)
        {
            var sql = @"select * from 'Transaction' t
                        left join Category c on c.Id = t.CategoryId
                        left join Account a on a.Email = c.Email
                        left join Icon i on i.Id = c.IconId
                        where c.Email=@email
                        Order by t.Date desc";
            var data = await Clients.Sql.QueryAsync<Transaction,Category, Account, Icon, Transaction>(sql, (transaction, category, account, icon) => {
                category.Account = account;
                category.Icon = icon;
                transaction.Category = category;
                return transaction;
            }, new { 
                email
            }, 
            splitOn: "Id,Email, Id");
            return data.Take(20).ToList();
        }

        public static async Task<List<Transaction>> ListTransactionByType(string email, string type)
        {
            var sql = @"select * from 'Transaction' t
                        left join Category c on c.Id = t.CategoryId
                        left join Account a on a.Email = c.Email
                        left join Icon i on i.Id = c.IconId
                        where c.Email=@email and c.Type=@type
                        Order by t.Date desc";
            var data = await Clients.Sql.QueryAsync<Transaction, Category, Account, Icon, Transaction>(sql, (transaction, category, account, icon) => {
                category.Account = account;
                category.Icon = icon;
                transaction.Category = category;
                return transaction;
            }, new
            {
                email,
                type
            }, splitOn: "Id,Email,Id");

            return data.ToList();
        }

        /// <summary>
        /// Get Transaction by Id
        /// </summary>
        /// <param name="email"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public static async Task<Transaction?> GetTransaction(string email, int transactionId)
        {
            var sql = @"select * from 'Transaction' t
                        left join Category c on c.Id = t.CategoryId
                        left join Account a on a.Email = c.Email
                        left join Icon i on i.Id = c.IconId
                        where c.Email=@email and t.Id=@transactionId";

            var data = await Clients.Sql.QueryAsync<Transaction, Category, Account, Icon, Transaction>(sql, (transaction, category, account, icon) => {
                category.Account = account;
                category.Icon = icon;
                transaction.Category = category;
                return transaction;
            }, new
            {
                email,
                transactionId
            }, splitOn: "Id,Email,Id");

            return data.FirstOrDefault();
        }

        /// <summary>
        /// Delete Transaction by Id
        /// </summary>
        /// <param name="TransactionId"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteTransaction(int TransactionId)
        {
            var sql = @"DELETE from ""Transaction"" where Id=@TransactionId";
            
            try
            {
                await Clients.Sql.ExecuteAsync(sql, new
                {
                    TransactionId
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> UpdateTransaction(int TransactionId, DateTime Date, string CategoryId, string Description, Double Amount)
        {
            var sql = @"UPDATE ""Transaction""
                        SET Date=@Date, CategoryId=@CategoryId, Description=@Description, Amount=@Amount
                        WHERE Id=@TransactionId;";

            try
            {
                await Clients.Sql.ExecuteAsync(sql, new
                {
                    Date,
                    CategoryId,
                    Description,
                    Amount,
                    TransactionId
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<List<Transaction>> ListTransactionByDate(string email, string type, DateTime DateFrom, DateTime DateTo)
        {
            var sql = @"select * from 'Transaction' t
                        left join Category c on c.Id = t.CategoryId
                        left join Account a on a.Email = c.Email
                        left join Icon i on i.Id = c.IconId
                        where c.Email=@email and c.Type=@type and t.Date>=@DateFrom and t.Date<=@DateTo
                        Order by t.Date desc";
            var data = await Clients.Sql.QueryAsync<Transaction, Category, Account, Icon, Transaction>(sql, (transaction, category, account, icon) => {
                category.Account = account;
                category.Icon = icon;
                transaction.Category = category;
                return transaction;
            }, new
            {
                email,
                type,
                DateFrom,
                DateTo
            }, splitOn: "Id,Email,Id");

            return data.ToList();
        }

        public static async Task<List<Transaction>> ListTransactionByDateAndCate(string email, string type, DateTime DateFrom, DateTime DateTo, List<int> ListId)
        {
            int[] CateIds = ListId.ToArray();
            var sql = @"select * from 'Transaction' t
                        left join Category c on c.Id = t.CategoryId
                        left join Account a on a.Email = c.Email
                        left join Icon i on i.Id = c.IconId
                        where c.Email=@email and c.Type=@type and t.Date>=@DateFrom and t.Date<=@DateTo and c.Id in @CateIds
                        Order by t.Date desc";
            var data = await Clients.Sql.QueryAsync<Transaction, Category, Account, Icon, Transaction>(sql, (transaction, category, account, icon) => {
                category.Account = account;
                category.Icon = icon;
                transaction.Category = category;
                return transaction;
            }, new
            {
                email,
                type,
                DateFrom,
                DateTo,
                CateIds
            }, splitOn: "Id,Email,Id");

            return data.ToList();
        }
    }
}
