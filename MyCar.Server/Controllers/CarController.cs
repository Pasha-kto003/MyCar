﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<int>> Post([FromBody] CarApi carApi)
        {
            var car = (Car)carApi;
            await dbContext.Cars.AddAsync(car);
            await dbContext.SaveChangesAsync();
            return Ok(car.Id);
        }

        // PUT api/<CarController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CarApi carApi)
        {
            var result = await dbContext.Cars.FindAsync(id);
            if (result == null)
                return NotFound();
            var car = (Car)carApi;
            dbContext.Entry(result).CurrentValues.SetValues(car);
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
