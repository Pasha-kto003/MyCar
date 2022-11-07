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

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<UserApi> Get()
        {
            return dbContext.Users.ToList().Select(s => {
                var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
                var type = dbContext.UserTypes.FirstOrDefault(t=> t.Id == s.UserTypeId);
                return CreateUserApi(s, passport, type);
            });
        }

        private UserApi CreateUserApi(User user, Passport passport, UserType userType)
        {
            var clientApi = (UserApi)user;
            clientApi.Passport = (PassportApi)passport;
            clientApi.UserType = (UserTypeApi)userType;
            return clientApi;
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserApi>> Get(long? id)
        {
            var client = await dbContext.Users.FindAsync(id);
            var passport = await dbContext.Passports.FindAsync(client.PassportId);
            var type = await dbContext.UserTypes.FindAsync(client.UserTypeId);
            return CreateUserApi(client, passport, type);
        }

[HttpGet("Type, Text")]
        public IEnumerable<UserApi> SearchByUser(string type, string text)
        {
            if (text == "")
            {
                return dbContext.Users.ToList().Select(s => {
                    var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
                    return CreateUserApi(s, passport);
                });
            }
            switch (type)
            {
                case "Логин":

                    return dbContext.Users.Where(s => s.UserName.ToLower().Contains(text)).ToList().Select(s =>
                    {
                        var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
                        var type = dbContext.UserTypes.FirstOrDefault(t => t.Id == s.UserTypeId);
                        return CreateUserApi(s, passport, type);
                    });

                    break;
                case "Фамилия":

                    return dbContext.Users.Where(s => s.Passport.LastName.ToLower().Contains(text)).ToList().Select(s =>
                    {
                        var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
                        var type = dbContext.UserTypes.FirstOrDefault(t => t.Id == s.UserTypeId);
                        return CreateUserApi(s, passport, type);
                    });

                    break;
                case "Email":

                    return dbContext.Users.Where(s => s.Email.ToLower().Contains(text)).ToList().Select(s =>
                    {
                        var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
                        var type = dbContext.UserTypes.FirstOrDefault(t => t.Id == s.UserTypeId);
                        return CreateUserApi(s, passport, type);
                    });

                    break;

                default:

                    return dbContext.Users.ToList().Select(s =>
                    {
                        var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
                        var type = dbContext.UserTypes.FirstOrDefault(t => t.Id == s.UserTypeId);
                        return CreateUserApi(s, passport, type);
                    });

                    break;
            }
        }


        // POST api/<UserController>
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<long>> Post([FromBody] UserApi userApi)
        {
            Passport passport = (Passport)userApi.Passport;
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
        public async Task<ActionResult> Put(long id, [FromBody] UserApi userApi)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            Passport passport = (Passport)userApi.Passport;
            if (passport.Id == 0)
                return BadRequest("Неверный паспорт");
            User newClient = (User)userApi;
            dbContext.Entry(user).CurrentValues.SetValues(newClient);
            user.Passport = passport;
            await dbContext.SaveChangesAsync();
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
