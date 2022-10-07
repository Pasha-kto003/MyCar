using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypeController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public UserTypeController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<UserTypeController>
        [HttpGet]
        public IEnumerable<UserTypeApi> Get()
        {
            return dbContext.UserTypes.Select(s => (UserTypeApi)s);
        }

        // GET api/<UserTypeController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserTypeApi>> Get(int id)
        {
            var result = await dbContext.UserTypes.FindAsync(id);
            if (result == null)
                return NotFound();
            return Ok((UserTypeApi)result);
        }

        // POST api/<UserTypeController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] UserTypeApi userType)
        {
            var type = (UserType)userType;
            await dbContext.UserTypes.AddAsync(type);
            await dbContext.SaveChangesAsync();
            return Ok(type.Id);
        }

        // PUT api/<UserTypeController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] UserTypeApi userType)
        {
            var result = await dbContext.UserTypes.FindAsync(id);
            if (result == null)
                return NotFound();
            var type = (UserType)userType;
            dbContext.Entry(result).CurrentValues.SetValues(type);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<UserTypeController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var type = await dbContext.UserTypes.FindAsync(id);
            if (type == null)
                return NotFound();
            dbContext.Remove(type);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
