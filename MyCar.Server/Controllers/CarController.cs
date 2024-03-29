﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsApi;
using MyCar.Server.DataModels;
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

        private CarApi GetCar(Car car)
        {
            return ModelData.GetCar(car, dbContext);
        }

        [HttpGet]
        public IEnumerable<CarApi> GetCars()
        {
            return dbContext.Cars.ToList().Select(s =>
            {
                return GetCar(s);
            });
        }

        // GET api/<CarController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CarApi>> Get(int id) //
        {
            var car = await dbContext.Cars.FindAsync(id);
            return GetCar(car);
        }

        [HttpGet("CarName")]
        public async Task<ActionResult<CarApi>> GetName(string name) // for test
        {
            var car = dbContext.Cars.FirstOrDefault(s => s.Model.ModelName.ToLower().Contains(name) || s.Model.Mark.MarkName.ToLower().Contains(name));

            return GetCar(car);
        }

        [HttpGet("Type, Text, Filter")]
        public IEnumerable<CarApi> SearchByCar(string type, string text, string filter)
        {
            IEnumerable<CarApi> CarsApi = dbContext.Cars.ToList().Select(s =>
            {
                return GetCar(s);
            });

            if (text != "$")
                switch (type)
                {
                    case "Модель":
                        CarsApi = CarsApi.Where(s => s.Model.ModelName.ToLower().Contains(text)).ToList();
                        break;
                    case "Название":
                        CarsApi = CarsApi.Where(s => s.CarName.ToLower().Contains(text)).ToList();
                        break;
                    case "Марка":
                        CarsApi = CarsApi.Where(s => s.CarMark.ToLower().Contains(text)).ToList();
                        break;
                    case "Цена":
                        CarsApi = CarsApi.Where(s => s.CarPrice.ToString().ToLower().Contains(text)).ToList();
                        break;
                    default:
                        CarsApi = CarsApi.ToList();
                        break;
                }

            if (filter != "Все")
            {
                int id = dbContext.MarkCars.FirstOrDefault(s=> s.MarkName.ToLower().Contains(filter)).Id;
                CarsApi = CarsApi.Where(s => s.Model.MarkId == id);
            }

            return CarsApi.ToList();
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
