using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public CarController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<CarController>
        [HttpGet]
        public IEnumerable<CarApi> Get()
        {
            return dbContext.Cars.Select(s => (CarApi)s);
        }

        // GET api/<CarController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CarApi>> Get(int id)
        {
            var result = await dbContext.Cars.FindAsync(id);
            if (result == null)
                return NotFound();
            return Ok((CarApi)result);
        }

        // POST api/<CarController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] CarApi value)
        {

            var newCar = (Car)value;
            dbContext.Cars.Add(newCar);
            await dbContext.SaveChangesAsync();
            var cross = value.CharacteristicCars.Select(s => (CharacteristicCar)s);
            await dbContext.CharacteristicCars.AddRangeAsync(cross.Select(s => new CharacteristicCar
            {
                CarId = newCar.Id,
                CharacteristicId = s.CharacteristicId,
                CharacteristicValue = s.CharacteristicValue
            }));
            await dbContext.SaveChangesAsync();
            return Ok(newCar.Id);
        }


        // PUT api/<CarController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CarApi editOrderOut)
        {

            var oldOrderOut = await dbContext.Cars.FindAsync(id);
            if (oldOrderOut == null)
                return NotFound();
            Car newOrderOut = (Car)editOrderOut;
            dbContext.Entry(oldOrderOut).CurrentValues.SetValues(newOrderOut);
            await dbContext.SaveChangesAsync();
            return Ok(); //
        }

        // DELETE api/<CarController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var car = await dbContext.Cars.FindAsync(id);
            if (car == null)
                return NotFound();
            dbContext.Remove(car);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
