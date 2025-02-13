using Microsoft.AspNetCore.Mvc;

namespace MyCellarApi.Controllers
{
    [Route("api/[controller]")]
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
