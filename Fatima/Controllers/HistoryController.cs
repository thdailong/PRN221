using Fatima.DAM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fatima.Controllers
{
    [Authorize]
    public class HistoryController : APIBase
    {
        [HttpGet]
        [Route("email={email}")]
        public async Task<ActionResult<object>> GetAllCateByEmail(string email)
        {
            var list = await DAM.CategoryDAM.ListCategory(email);
            return list;

        }

        [HttpGet]
        [Route("transaction/email={email}&type={type}")]
        public async Task<ActionResult<object>> GetAllTransactionByType(string email, string type)
        {
            var list = await DAM.TransactionDAM.ListTransactionByType(email, type);
            return list;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetCate([FromQuery] string email, [FromQuery] string categoryId)
        {
            var category = await DAM.CategoryDAM.GetCategory(email, categoryId);
            if (category == null) return new {
                title = "Category was not found",
                email = email,
            };
            return category;
        }
    }
}
