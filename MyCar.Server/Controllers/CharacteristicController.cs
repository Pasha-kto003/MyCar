﻿using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacteristicController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public CharacteristicController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<CharacteristicController>
        [HttpGet]
        public IEnumerable<CharacteristicApi> Get() //
        {
            return dbContext.Characteristics.ToList().Select(s =>
            {
                return GetCharacteristicApi(s);
            });
        }

        private CharacteristicApi GetCharacteristicApi(Characteristic characteristic)
        {
            var result = (CharacteristicApi)characteristic;
            var unit = dbContext.Units.FirstOrDefault(s=> s.Id == characteristic.UnitId);
            result.Unit = (UnitApi)unit;
            return result;
        }

        // GET api/<CharacteristicController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CharacteristicApi>> Get(int id)
        {
            var characteristic = await dbContext.Characteristics.FindAsync(id);
            if (characteristic == null)
                return NotFound();
            return GetCharacteristicApi(characteristic);
        }

        [HttpGet("Type, Text, Filter")]
        public IEnumerable<CharacteristicApi> SearchCharacteristic(string type, string text, string filter)
        {
            IEnumerable<CharacteristicApi> CharacteristicsApi = dbContext.Characteristics.ToList().Select(s =>
            {
                return GetCharacteristicApi(s);
            });

            if (text != "$")
                switch (type)
                {
                    case "Характеристика":
                        CharacteristicsApi = CharacteristicsApi.Where(s => s.CharacteristicName.ToLower().Contains(text)).ToList();
                        break;
                    default:
                        CharacteristicsApi = CharacteristicsApi.ToList();
                        break;
                }

            if (filter != "Все")
            {
                CharacteristicsApi = CharacteristicsApi.Where(s => s.Unit.UnitName.Contains(filter)).ToList();
            }

            return CharacteristicsApi.ToList();
        }

        // POST api/<CharacteristicController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] CharacteristicApi characteristicApi)
        {
            var characteristic = (Characteristic)characteristicApi;
            await dbContext.Characteristics.AddAsync(characteristic);
            await dbContext.SaveChangesAsync();
            return Ok(characteristic.Id);
        }

        // PUT api/<CharacteristicController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CharacteristicApi characteristicApi)
        {
            var characteristic = await dbContext.Characteristics.FindAsync(id);
            if (characteristic == null)
                return NotFound();
            dbContext.Entry(characteristic).CurrentValues.SetValues(characteristicApi);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<CharacteristicController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var characteristic = await dbContext.Characteristics.FindAsync(id);
            if (characteristic == null)
                return NotFound();
            dbContext.Characteristics.Remove(characteristic);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
