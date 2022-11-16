using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public UserController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private UserApi GetUser(User user)
        {
            var result = (UserApi)user;
            var type = dbContext.UserTypes.FirstOrDefault(s=> s.Id == user.UserTypeId);
            result.UserType = (UserTypeApi)type;
            var passport = dbContext.Passports.FirstOrDefault(s=> s.Id == user.PassportId);
            result.Passport = (PassportApi)passport;
            return result;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<UserApi> Get()
        {
            return dbContext.Users.ToList().Select(s => {
                return GetUser(s);
            });
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserApi>> Get(long? id)
        {
            var client = await dbContext.Users.FindAsync(id);
            return GetUser(client);
        }

        [HttpGet("Type, Text, Filter")]
        public IEnumerable<UserApi> SearchByUser(string type, string text, string filter)
        {
            IEnumerable<UserApi> UsersApi = dbContext.Users.ToList().Select(s =>
            {
                return GetUser(s);
            });

            if (text != "$")
                switch (type)
                {
                    case "Логин":
                        UsersApi = UsersApi.Where(s => s.UserName.ToLower().Contains(text)).ToList();
                        break;
                    case "Фамилия":
                        UsersApi = UsersApi.Where(s => s.Passport.LastName.ToLower().Contains(text)).ToList();
                        break;
                    case "Email":
                        UsersApi = UsersApi.Where(s => s.Email.ToLower().Contains(text)).ToList();
                        break;
                    default:
                        UsersApi = UsersApi.ToList();
                        break;
                }

            if (filter != "Все")
            {
                UsersApi = UsersApi.Where(s => s.UserType.TypeName.Contains(filter)).ToList();
            }

            return UsersApi.ToList();
        }


        // POST api/<UserController>
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<long>> Post([FromBody] UserApi userApi)
        {
            Passport passport = new Passport();
            passport.Id = dbContext.Passports.Count() + 1;
            await dbContext.Passports.AddAsync(passport);
            await dbContext.SaveChangesAsync();
            User newUser = (User)userApi;
            newUser.PassportId = passport.Id;
            await dbContext.Users.AddAsync(newUser);
            await dbContext.SaveChangesAsync();
            return Ok(newUser.Id);
        }

        // PUT api/<UserController>/5
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] UserApi userApi)
        {
            var result = await dbContext.Users.FindAsync(id);
            if(result == null)
            {
                return NotFound();
            }
            var user = (User)userApi;
            dbContext.Users.Update(result).CurrentValues.SetValues(user);
            dbContext.SaveChanges();
            return Ok();
        }

        // DELETE api/<UserController>/5
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var oldUser = await dbContext.Users.FindAsync(id);
            if (oldUser == null)
                return NotFound();
            dbContext.Users.Remove(oldUser);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
