using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public EquipmentController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<EquipmentController>
        [HttpGet]
        public IEnumerable<EquipmentApi> Get()
        {
            return dbContext.Equipment.Select(s => (EquipmentApi)s);
        }

        // GET api/<EquipmentController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentApi>> Get(int id)
        {
            var result = await dbContext.Equipment.FindAsync(id);
            if (result == null)
                return NotFound();
            return Ok((EquipmentApi)result);
        }

        [HttpGet("Type, Text")]
        public IEnumerable<EquipmentApi> SearchEquipment(string type, string text)
        {
            if (text == "")
            {
                return dbContext.Equipment.ToList().Select(s => (EquipmentApi)s);
            }
            switch (type)
            {
                case "Комплектация":
                    return dbContext.Equipment.Where(s => s.NameEquipment.ToLower().Contains(text)).Select(s => (EquipmentApi)s);
                    break;
                case "Цена":
                    return dbContext.Equipment.Where(s => s.NameEquipment.ToLower().Contains(text)).Select(s => (EquipmentApi)s);
                    break;
                default:
                    return dbContext.Equipment.ToList().Select(s => (EquipmentApi)s);
                    break;
            }
        }

        // POST api/<EquipmentController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] EquipmentApi equipmentApi)
        {
            var equipment = (Equipment)equipmentApi;
            await dbContext.Equipment.AddAsync(equipment);
            await dbContext.SaveChangesAsync();
            return Ok(equipment.Id);
        }

        // PUT api/<EquipmentController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] EquipmentApi equipmentApi)
        {
            var result = await dbContext.Equipment.FindAsync(id);
            if (result == null)
                return NotFound();
            var equipment = (Equipment)equipmentApi;
            dbContext.Entry(result).CurrentValues.SetValues(equipment);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<EquipmentController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var equipment = await dbContext.Equipment.FindAsync(id);
            if (equipment == null)
                return NotFound();
            dbContext.Remove(equipment);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
