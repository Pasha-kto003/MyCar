using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionTypeController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public ActionTypeController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<ActionTypeController>
        [HttpGet]
        public IEnumerable<ActionTypeApi> Get()
        {
            return dbContext.ActionTypes.ToList().Select(s => (ActionTypeApi)s);
        }

        // GET api/<ActionTypeController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ActionTypeApi>> Get(int id)
        {
            var action = await dbContext.ActionTypes.FindAsync(id);
            if (action == null)
                return NotFound();
            //return Ok(CreateUnitApi(unit, products));
            return Ok((ActionTypeApi)action);
        }

        // POST api/<ActionTypeController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] ActionTypeApi typeApi)
        {
            var newtype = (ActionType)typeApi;
            await dbContext.ActionTypes.AddAsync(newtype);
            await dbContext.SaveChangesAsync();
            return Ok(newtype.Id);
        }
    }
}
