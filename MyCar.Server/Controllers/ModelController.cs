﻿using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public ModelController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<ModelController>
        [HttpGet]
        public IEnumerable<ModelApi> Get()
        {
            return dbContext.Models.Select(s => (ModelApi)s);
        }

        private ModelApi GetModelApi(ModelApi model)
        {
            var result = (ModelApi)model;
            var mark = dbContext.MarkCars.FirstOrDefault(s => s.Id == model.MarkId);
            result.MarkCar = (MarkCarApi)mark;
            return result;
        }

        // GET api/<ModelController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModelApi>> Get(int id)
        {
            var result = await dbContext.Models.FindAsync(id);
            if (result == null)
                return NotFound();
            return Ok((ModelApi)result);
        }

        [HttpGet("Type, Text")]
        public IEnumerable<ModelApi> SearchModel(string type, string text)
        {
            if (text == "")
            {
                return dbContext.Models.ToList().Select(s => (ModelApi)s);
            }
            switch (type)
            {
                case "Наименование":

                    return dbContext.Models.Where(s => s.ModelName.ToLower().Contains(text)).Select(s => (ModelApi)s);
                    break;

                default:
                    return dbContext.Models.ToList().Select(s => (ModelApi)s);
                    break;
            }
        }

        // POST api/<ModelController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] ModelApi modelApi)
        {
            var model = (Model)modelApi;
            await dbContext.Models.AddAsync(model);
            await dbContext.SaveChangesAsync();
            return Ok(model.Id);
        }

        // PUT api/<ModelController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ModelApi modelApi)
        {
            var result = await dbContext.Models.FindAsync(id);
            if (result == null)
                return NotFound();
            var model = (Model)modelApi;
            dbContext.Entry(result).CurrentValues.SetValues(model);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<ModelController>/5
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
