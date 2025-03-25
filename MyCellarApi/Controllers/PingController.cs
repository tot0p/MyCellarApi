using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MyCellarApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("{\"message\":\"Pong !\"}");
        }

    }
}
