using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyCellarApi.Data;
using MyCellarApi.Models;
using MyCellarApiCore.Controllers;

namespace MyCellarApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CellarsController : BaseController<MyCellarDbContext, Cellar>
    {
        public CellarsController(MyCellarDbContext context) : base(context)
        {
        }
    }
}
