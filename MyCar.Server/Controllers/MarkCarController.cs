using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkCarController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public MarkCarController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<MarkCarController>
        [HttpGet]
        public IEnumerable<MarkCarApi> Get()
        {
            return dbContext.MarkCars.ToList().Select(s => {
                return GetMark(s);
            });
        }
        private MarkCarApi GetMark(MarkCar mark)
        {
            var result = (MarkCarApi)mark;
            var models = dbContext.Models.Where(t => t.MarkId == mark.Id).Select(t => (ModelApi)t).ToList();
            result.Models = models;
            return result;
        }

        // GET api/<MarkCarController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MarkCarApi>> Get(int id)
        {
            var result = await dbContext.MarkCars.FindAsync(id);
            if (result == null)
                return NotFound();
            return Ok(GetMark(result));
        }

        [HttpGet("Type, Text")]
        public IEnumerable<MarkCarApi> SearchByMark(string type, string text)
        {
            if (text == "")
            {
                return dbContext.MarkCars.ToList().Select(s => {
                    return GetMark(s);
                });
            }
            switch (type)
            {
                case "Марка":

                    return dbContext.MarkCars.Where(s => s.MarkName.ToLower().Contains(text)
                    || s.Models.Any(n=>n.ModelName.ToLower().Contains(text))).ToList().Select(s =>
                    {
                        return GetMark(s);
                    });

                    break;

                default:
                    return dbContext.MarkCars.ToList().Select(s => {
                        return GetMark(s);
                    });

                    break;
            }
        }

        // POST api/<MarkCarController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] MarkCarApi markCarApi)
        {
            var mark = (MarkCar)markCarApi;
            await dbContext.MarkCars.AddAsync(mark);
            await dbContext.SaveChangesAsync();
            return Ok(mark.Id);
        }

        // PUT api/<MarkCarController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] MarkCarApi markCarApi)
        {
            var result = await dbContext.MarkCars.FindAsync(id);
            if (result == null)
                return NotFound();
            var mark = (MarkCar)markCarApi;
            dbContext.Entry(result).CurrentValues.SetValues(mark);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<MarkCarController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var mark = await dbContext.MarkCars.FindAsync(id);
            if (mark == null)
                return NotFound();
            dbContext.Remove(mark);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
