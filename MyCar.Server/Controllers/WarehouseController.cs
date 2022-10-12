using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public WarehouseController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<WarehouseController>
        [HttpGet]
        public IEnumerable<WareHouseApi> Get()
        {
            return dbContext.Warehouses.Select(s => (WareHouseApi)s);
        }

        // GET api/<WarehouseController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WareHouseApi>> Get(int id)
        {
            var result = await dbContext.Warehouses.FindAsync(id);
            if (result == null)
                return NotFound();
            return Ok((WareHouseApi)result);
        }

        // POST api/<WarehouseController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] WareHouseApi wareHouse)
        {
            var warehouse = (Warehouse)wareHouse;
            await dbContext.Warehouses.AddAsync(warehouse);
            await dbContext.SaveChangesAsync();
            return Ok(warehouse.Id);
        }

        // PUT api/<WarehouseController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] WareHouseApi wareHouse)
        {
            var result = await dbContext.Warehouses.FindAsync(id);
            if (result == null)
                return NotFound();
            var wareHouseApi = (Warehouse)wareHouse;
            dbContext.Entry(result).CurrentValues.SetValues(wareHouseApi);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<WarehouseController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await dbContext.Models.FindAsync(id);
            if (model == null)
                return NotFound();
            dbContext.Remove(model);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }

}
