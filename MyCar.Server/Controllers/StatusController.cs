using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public StatusController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<StatusController>
        [HttpGet]
        public IEnumerable<StatusApi> Get()
        {
            return dbContext.Statuses.ToList().Select(s => (StatusApi)s);
        }

        // GET api/<StatusController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusApi>> Get(int id)
        {
            var status = await dbContext.Statuses.FindAsync(id);
            if (status == null)
                return NotFound();
            return Ok((StatusApi)status);
        }

        // POST api/<StatusController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] StatusApi statusApi)
        {
            var newstatus = (Status)statusApi;
            await dbContext.Statuses.AddAsync(newstatus);
            await dbContext.SaveChangesAsync();
            return Ok(newstatus.Id);
        }
    }
}
