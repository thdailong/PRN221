using Microsoft.AspNetCore.Mvc;

namespace Finance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class APIBase : ControllerBase
    {
    }
}
