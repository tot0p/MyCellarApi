using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyCellarApi.Data;
using MyCellarApi.Models;
using MyCellarApiCore.Controllers;

namespace MyCellarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CellarsController : BaseController<MyCellarDbContext, Cellar>
    {
        public CellarsController(MyCellarDbContext context) : base(context)
        {
        }
    }
}
