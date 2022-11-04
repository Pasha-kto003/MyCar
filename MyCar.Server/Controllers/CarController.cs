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

        private CarApi GetCross(Car car, List<CharacteristicCarApi> characteristics, Model model, MarkCar mark, Equipment equipment, BodyType bodyType)
        {
            var result = (CarApi)car;
            result.CharacteristicCars = characteristics;
            result.Model = (ModelApi)model;
            result.Model.MarkCar = (MarkCarApi)mark;
            result.Equipment = (EquipmentApi)equipment;
            result.BodyType = (BodyTypeApi)bodyType;
            return result;
        }

        [HttpGet]
        public IEnumerable<CarApi> GetCars()
        {
            return dbContext.Cars.ToList().Select(s =>
            {
                var characteristics = dbContext.CharacteristicCars.Where(t => t.CarId == s.Id).Select(t => (CharacteristicCarApi)t).ToList();
                var model = dbContext.Models.FirstOrDefault(t => t.Id == s.ModelId);
                var mark = dbContext.MarkCars.FirstOrDefault(i => i.Id == model.MarkId);
                var equipment = dbContext.Equipment.FirstOrDefault(e => e.Id == s.EquipmentId);
                var body = dbContext.BodyTypes.FirstOrDefault(b => b.Id == s.TypeId);
                return GetCross(s, characteristics, model, mark, equipment, body);
            });
        }

        // GET api/<CarController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CarApi>> Get(int id)
        {
            var car = await dbContext.Cars.FindAsync(id);
            var characteristics = dbContext.CharacteristicCars.Where(t => t.CarId == id).Select(t => (CharacteristicCarApi)t).ToList();
            var model = dbContext.Models.FirstOrDefault(t => t.Id == car.ModelId);
            var mark = dbContext.MarkCars.FirstOrDefault(i => i.Id == model.MarkId);
            var equipment = dbContext.Equipment.FirstOrDefault(e => e.Id == car.EquipmentId);
            var body = dbContext.BodyTypes.FirstOrDefault(b => b.Id == car.TypeId);
            return GetCross(car, characteristics, model, mark, equipment, body);
        }

        [HttpGet("Type, Text")]
        public IEnumerable<CarApi> SearchByCar(string type, string text)
        {
            switch (type)
            {
                case "Артикул":

                    return dbContext.Cars.Where(s=> s.Articul.Contains(text)).ToList().Select(s =>
                    {
                        var characteristics = dbContext.CharacteristicCars.Where(t => t.CarId == s.Id).Select(t => (CharacteristicCarApi)t).ToList();
                        var model = dbContext.Models.FirstOrDefault(t => t.Id == s.ModelId);
                        var mark = dbContext.MarkCars.FirstOrDefault(i => i.Id == model.MarkId);
                        var equipment = dbContext.Equipment.FirstOrDefault(e => e.Id == s.EquipmentId);
                        var body = dbContext.BodyTypes.FirstOrDefault(b => b.Id == s.TypeId);
                        return GetCross(s, characteristics, model, mark, equipment, body);
                    });

                    break;
                case "Модель":

                    return dbContext.Cars.Where(s => s.Model.ModelName.ToLower().Contains(text)).ToList().Select(s =>
                    {
                        var characteristics = dbContext.CharacteristicCars.Where(t => t.CarId == s.Id).Select(t => (CharacteristicCarApi)t).ToList();
                        var model = dbContext.Models.FirstOrDefault(t => t.ModelName.ToLower().Contains(text)); 
                        var mark = dbContext.MarkCars.FirstOrDefault(i => i.Id == model.MarkId);
                        var equipment = dbContext.Equipment.FirstOrDefault(e => e.Id == s.EquipmentId);
                        var body = dbContext.BodyTypes.FirstOrDefault(b => b.Id == s.TypeId);
                        return GetCross(s, characteristics, model, mark, equipment, body);
                    });

                    break;
                case "Марка":

                    return dbContext.Cars.Where(s => s.Model.Mark.MarkName.ToLower().Contains(text)).ToList().Select(s => 
                    {
                        var characteristics = dbContext.CharacteristicCars.Where(t => t.CarId == s.Id).Select(t => (CharacteristicCarApi)t).ToList();
                        var model = dbContext.Models.FirstOrDefault(t => t.Id == s.ModelId);
                        var mark = dbContext.MarkCars.FirstOrDefault(i => i.MarkName.ToLower().Contains(text));
                        var equipment = dbContext.Equipment.FirstOrDefault(e => e.Id == s.EquipmentId);
                        var body = dbContext.BodyTypes.FirstOrDefault(b => b.Id == s.TypeId);
                        return GetCross(s, characteristics, model, mark, equipment, body);
                    });

                    break;

                case "Цена":

                    return dbContext.Cars.Where(s => s.CarPrice.ToString().ToLower().Contains(text)).ToList().Select(s => 
                    {
                        var characteristics = dbContext.CharacteristicCars.Where(t => t.CarId == s.Id).Select(t => (CharacteristicCarApi)t).ToList();
                        var model = dbContext.Models.FirstOrDefault(t => t.Id == s.ModelId);
                        var mark = dbContext.MarkCars.FirstOrDefault(i => i.Id == model.MarkId);
                        var equipment = dbContext.Equipment.FirstOrDefault(e => e.Id == s.EquipmentId);
                        var body = dbContext.BodyTypes.FirstOrDefault(b => b.Id == s.TypeId);
                        return GetCross(s, characteristics, model, mark, equipment, body);
                    });
                    break;

                default:

                    return dbContext.Cars.ToList().Select(s => 
                    {
                        var characteristics = dbContext.CharacteristicCars.Where(t => t.CarId == s.Id).Select(t => (CharacteristicCarApi)t).ToList();
                        var model = dbContext.Models.FirstOrDefault(t => t.Id == s.ModelId);
                        var mark = dbContext.MarkCars.FirstOrDefault(i => i.Id == model.MarkId);
                        var equipment = dbContext.Equipment.FirstOrDefault(e => e.Id == s.EquipmentId);
                        var body = dbContext.BodyTypes.FirstOrDefault(b => b.Id == s.TypeId);
                        return GetCross(s, characteristics, model, mark, equipment, body);
                    });
                    break;
            }
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
