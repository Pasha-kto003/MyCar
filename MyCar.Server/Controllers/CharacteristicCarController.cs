using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacteristicCarController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public CharacteristicCarController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
       
        // GET: api/<CharacteristicCarController>
        [HttpGet]
        public IEnumerable<CharacteristicCarApi> Get()
        {
            return dbContext.CharacteristicCars.Select(s => (CharacteristicCarApi)s);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CharacteristicCarApi edit)
        {
            var oldcross = await dbContext.CharacteristicCars.FindAsync(id);
            if (oldcross == null)
                return NotFound();
            dbContext.Entry(oldcross).CurrentValues.SetValues(dbContext);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
