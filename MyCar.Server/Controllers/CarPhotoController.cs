using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarPhotoController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public CarPhotoController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/<CarPhotoController>
        [HttpGet]
        public IEnumerable<CarPhotoApi> Get()
        {
            return dbContext.CarPhotos.ToList().Select(s => (CarPhotoApi)s);
        }

        // GET api/<CarPhotoController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CarPhotoApi>> Get(int id)
        {
            var result = await dbContext.CarPhotos.FindAsync(id);
            if (result == null)
                return NotFound();
            return Ok((CarPhotoApi)result);
        }

        // POST api/<CarPhotoController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] CarPhotoApi carPhotoApi)
        {
            var photo = (CarPhoto)carPhotoApi;
            await dbContext.CarPhotos.AddAsync(photo);
            await dbContext.SaveChangesAsync();
            return Ok(photo.Id);
        }

        // PUT api/<CarPhotoController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CarPhotoApi carPhotoApi)
        {
            var result = await dbContext.CarPhotos.FindAsync(id);
            if (result == null)
                return NotFound();
            var photo = (CarPhoto)carPhotoApi;
            dbContext.Entry(result).CurrentValues.SetValues(photo);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<CarPhotoController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var photo = await dbContext.CarPhotos.FindAsync(id);
            if (photo == null)
                return NotFound();
            dbContext.CarPhotos.Remove(photo);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
