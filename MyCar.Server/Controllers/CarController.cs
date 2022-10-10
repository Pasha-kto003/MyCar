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

        private CarApi CreateCarApi(Car car, List<CharacteristicApi> characteristics)
        {
            var result = (CarApi)car;
            result.Characteristics = characteristics;
            return result;
        }

        // GET: api/<CarController>
        [HttpGet]
        public IEnumerable<CarApi> Get()
        {
            return dbContext.Cars.ToList().Select(s =>
            {
                var characteristics = dbContext.CharacteristicCars.Where(t => t.CarId == s.Id).Select(t => (CharacteristicApi)t.Characteristic).ToList();
                return CreateCarApi(s, characteristics);
            });
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
        public async Task<ActionResult> Put(int id, [FromBody] CarApi editcar)
        {
            foreach (var characteristics in editcar.Characteristics)
                if (characteristics.ID == 0)
                    return BadRequest($"Продукт {characteristics.CharacteristicName} не существует");
            var car = (Car)editcar;
            var cross = dbContext.CharacteristicCars.FirstOrDefault(t => t.CarId == car.Id);
            var oldcar = await dbContext.Cars.FindAsync(id);
            if (oldcar == null)
                return NotFound();
            dbContext.Entry(oldcar).CurrentValues.SetValues(car);
            var characteristicRemove = dbContext.CharacteristicCars.Where(s => s.CarId == id).ToList();
            dbContext.CharacteristicCars.RemoveRange(characteristicRemove);
            var crosses = editcar.CharacteristicCars.Select(s => (CharacteristicCar)s);
            await dbContext.CharacteristicCars.AddRangeAsync(crosses.Select(s => new CharacteristicCar
            {
                CarId = car.Id,
                CharacteristicId = s.CharacteristicId,
                CharacteristicValue = s.CharacteristicValue
            }));
            await dbContext.SaveChangesAsync();
            return Ok();
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
