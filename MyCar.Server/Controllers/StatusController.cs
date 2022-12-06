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
        // GET: api/<BodyTypeController>
        [HttpGet]
        public IEnumerable<StatusApi> Get()
        {
            return dbContext.Statuses.Select(s => (StatusApi)s);
        }

        // GET api/<BodyTypeController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusApi>> Get(int id)
        {
            var result = await dbContext.Statuses.FindAsync(id);
            if (result == null)
                return NotFound();
            return Ok((StatusApi)result);
        }

        // POST api/<BodyTypeController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] StatusApi status)
        {
            var statusdb = (Status)status;
            await dbContext.Statuses.AddAsync(statusdb);
            await dbContext.SaveChangesAsync();
            return Ok(statusdb.Id);
        }

        // PUT api/<BodyTypeController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] StatusApi status)
        {
            var result = await dbContext.Statuses.FindAsync(id);
            if (result == null)
                return NotFound();
            var type = (StatusApi)status;
            dbContext.Entry(result).CurrentValues.SetValues(type);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Type, Text")]
        public IEnumerable<StatusApi> SearchBodyType(string type, string text)
        {
            if (type == null)
            {
                return dbContext.Statuses.Select(s => (StatusApi)s);
            }

            else if (type == "Статус")
            {
                return dbContext.Statuses.Where(s => s.StatusName.ToLower().Contains(text)).Select(t => (StatusApi)t);
            }

            else
            {
                return dbContext.Statuses.Select(s => (StatusApi)s);
            }
        }

        // DELETE api/<BodyTypeController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var status = await dbContext.Statuses.FindAsync(id);
            if (status == null)
                return NotFound();
            dbContext.Statuses.Remove(status);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
