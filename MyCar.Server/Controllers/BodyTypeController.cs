using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodyTypeController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public BodyTypeController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<BodyTypeController>
        [HttpGet]
        public IEnumerable<BodyTypeApi> Get()
        {
            return dbContext.BodyTypes.Select(s => (BodyTypeApi)s);
        }

        // GET api/<BodyTypeController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BodyTypeApi>> Get(int id)
        {
            var result = await dbContext.BodyTypes.FindAsync(id);
            if (result == null)
                return NotFound();
            return Ok((BodyTypeApi)result);
        }

        // POST api/<BodyTypeController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] BodyTypeApi bodyTypeApi)
        {
            var type = (BodyType)bodyTypeApi;
            await dbContext.BodyTypes.AddAsync(type);
            await dbContext.SaveChangesAsync();
            return Ok(type.Id);
        }

        // PUT api/<BodyTypeController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] BodyTypeApi bodyTypeApi)
        {
            var result = await dbContext.BodyTypes.FindAsync(id);
            if (result == null)
                return NotFound();
            var type = (BodyType)bodyTypeApi;
            dbContext.Entry(result).CurrentValues.SetValues(type);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<BodyTypeController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var body = await dbContext.BodyTypes.FindAsync(id);
            if (body == null)
                return NotFound();
            dbContext.BodyTypes.Remove(body);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
