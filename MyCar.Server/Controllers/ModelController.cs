using Microsoft.AspNetCore.Mvc;
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

        private List<ModelApi> GetModelApi(MarkCar mark, List<ModelApi> models)
        {
            var result = (MarkCarApi)mark;
            result.Models = models;
            return result.Models;
        }

        [HttpGet("Mark")]
        public IEnumerable<ModelApi> GetModel(string markName)
        {
            var models = dbContext.Models.ToList();
            var markscar = dbContext.MarkCars.FirstOrDefault(s => s.MarkName.ToLower().Contains(markName));
            if (markscar == null)
            {
                models = null;
            }

            var sortModels = models.Where(s => s.MarkId == markscar.Id).Select(t => (ModelApi)t).ToList();
            return GetModelApi(markscar, sortModels);
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
