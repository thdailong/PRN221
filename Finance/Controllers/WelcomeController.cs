using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Controllers
{
    [Authorize]
    public class WelcomeController : APIBase
    {
        [HttpGet]
        public ActionResult<object> Get()
        {
            return new
            {
                greeting = "Welcome to Finance API service",
                version = "1.0"
            };
        }

        [HttpGet("test")]
        public ActionResult<object> TestApi()
        {
            return new
            {
                greeting = "Finance API test page",
            };
        }
    }
}
