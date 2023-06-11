using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fatima.Controllers
{
    [Authorize]
    public class WelcomeController : APIBase
    {
        [HttpGet]
        public ActionResult<object> Get()
        {
            return new
            {
                greeting = "Welcome to Fatima API service",
                version = "1.0"
            };
        }

        [HttpGet("test")]
        public ActionResult<object> TestApi()
        {
            return new
            {
                greeting = "Fatima API test page",
            };
        }
    }
}
