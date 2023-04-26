using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DataModels;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountChangeHistoryController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public CountChangeHistoryController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private CountChangeHistoryApi GetWareHouseApi(CountChangeHistory countChange)
        {
            return ModelData.GetCount(countChange, dbContext);
        }
        // GET: api/<CountChangeHistoryController>
        [HttpGet]
        public IEnumerable<CountChangeHistoryApi> Get()
        {
            return dbContext.CountChangeHistories.ToList().Select(s =>
            {
                return GetWareHouseApi(s);
            });
        }

        // GET api/<CountChangeHistoryController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountChangeHistoryApi>> Get(int id)
        {
            var result = await dbContext.CountChangeHistories.FindAsync(id);
            if (result == null)
                return NotFound();
            return Ok((CountChangeHistoryApi)result);
        }

        // POST api/<CountChangeHistoryController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] CountChangeHistoryApi countChange)
        {
            var count = (CountChangeHistory)countChange;
            await dbContext.CountChangeHistories.AddAsync(count);
            await dbContext.SaveChangesAsync();
            return Ok(count.Id);
        }

        // PUT api/<CountChangeHistoryController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CountChangeHistoryApi countChange)
        {
            var result = await dbContext.CountChangeHistories.FindAsync(id);
            if (result == null)
                return NotFound();
            var countApi = (CountChangeHistory)countChange;
            dbContext.Entry(result).CurrentValues.SetValues(countApi);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<CountChangeHistoryController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await dbContext.CountChangeHistories.FindAsync(id);
            if (result == null)
                return NotFound();
            dbContext.Remove(result);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
