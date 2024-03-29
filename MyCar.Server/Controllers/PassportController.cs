﻿using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassportController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public PassportController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<PassportController>
        [HttpGet]
        public IEnumerable<PassportApi> Get()
        {
            return dbContext.Passports.Select(s => (PassportApi)s).ToList();
        }

        // POST api/<PassportController>
        [HttpPost]
        public async Task<ActionResult<long>> Post([FromBody] PassportApi passport)
        {
            var newPassport = (Passport)passport;
            await dbContext.Passports.AddAsync(newPassport);
            await dbContext.SaveChangesAsync();
            return Ok(newPassport.Id);
        }

        // PUT api/<PassportController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] PassportApi value)
        {
            var result = await dbContext.Passports.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var passport = (Passport)value;
            dbContext.Passports.Update(result).CurrentValues.SetValues(passport);
            dbContext.SaveChanges();
            return Ok();
        }

        // DELETE api/<PassportController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
