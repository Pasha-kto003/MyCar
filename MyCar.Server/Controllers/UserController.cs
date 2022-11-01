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
                return CreateUserApi(s, passport);
            });
        }

        private UserApi CreateUserApi(User user, Passport passport)
        {
            var clientApi = (UserApi)user;
            clientApi.Passport = (PassportApi)passport;
            return clientApi;
        }

        [HttpGet("UserName, Password")]
        public async Task<ActionResult<UserApi>> Enter(string userName, byte[] Password)
        {
            var user = dbContext.Users.FirstOrDefault(s => s.UserName == userName && s.PasswordHash == Password);
            if (user == null)
            {
                NotFound();
            }
            return (UserApi)user;
        }

        [HttpPost("UserName, Password, FirstName, Email")]
        public async Task<ActionResult<long>> Registration(string UserName, byte[] Password, string Email, string FirstName, int? PassportId)
        {
            var newUser = new User();
            await dbContext.SaveChangesAsync();
            newUser.UserName = UserName;
            newUser.PasswordHash = Password;
            newUser.Email = Email;
            newUser.PassportId = PassportId;
            await dbContext.Users.AddAsync(newUser);
            await dbContext.SaveChangesAsync();
            return Ok(newUser.Id);
        }

        //[HttpGet("UserName")]
        //public IEnumerable<UserApi> SearchByUserName(string userName)
        //{
        //    return dbContext.Users.Where(s => s.Username == userName).ToList().Select(s => {
        //        var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
        //        return CreateUserApi(s, passport);
        //    });
        //}

        //[HttpGet("Password")]
        //public IEnumerable<UserApi> SearchByUserPassword(string password)
        //{
        //    return dbContext.Users.Where(s=> s.Password == password).ToList().Select(s => {
        //        var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
        //        return CreateUserApi(s, passport);
        //    });
        //}

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserApi>> Get(long? id)
        {
            var client = await dbContext.Users.FindAsync(id);
            var passport = await dbContext.Passports.FindAsync(client.PassportId);
            return CreateUserApi(client, passport);
        }

        //[HttpGet("Password, UserName, FirstName, LastName, Patronymic, Telephone, Email")]
        //public IEnumerable<UserApi> SearchByUser(int type, string text)
        //{
        //    if (type == 1)
        //    {
        //        return dbContext.Users.Where(s => s.UserName.ToLower().Contains(text)).ToList().Select(s => {
        //            var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
        //            return CreateUserApi(s, passport);
        //        });
        //    }
        //    if (type == 2)
        //    {
        //        return dbContext.Users.Where(s => s.Passport.FirstName.ToLower().Contains(text)).ToList().Select(s =>
        //        {
        //            var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
        //            return CreateUserApi(s, passport);
        //        });
        //    }
        //    if (type == 3)
        //    {
        //        return dbContext.Users.Where(s => s.Passport.LastName.ToLower().Contains(text)).ToList().Select(s =>
        //        {
        //            var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
        //            return CreateUserApi(s, passport);
        //        });
        //    }
        //    if (type == 4)
        //    {
        //        return dbContext.Users.Where(s => s.Passport.Patronymic.ToLower().Contains(text)).ToList().Select(s =>
        //        {
        //            var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
        //            return CreateUserApi(s, passport);
        //        });
        //    }
        //    if (type == 5)
        //    {
        //        return dbContext.Users.Where(s => s.Email.ToLower().Contains(text)).ToList().Select(s => {
        //            var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
        //            return CreateUserApi(s, passport);
        //        });
        //    }
        //    else
        //    {
        //        return dbContext.Users.ToList().Select(s => {
        //            var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
        //            return CreateUserApi(s, passport);
        //        });
        //    }
        //}

        [HttpGet("Password, UserName, FirstName, LastName, Patronymic, Telephone, Email")]
        public IEnumerable<UserApi> SearchByUser(string type, string text)
        {
            switch (type)
            {
                case "Логин":

                    return dbContext.Users.Where(s => s.UserName.ToLower().Contains(text)).ToList().Select(s =>
                    {
                        var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
                        return CreateUserApi(s, passport);
                    });

                    break;
                case "Фамилия":

                    return dbContext.Users.Where(s => s.Passport.LastName.ToLower().Contains(text)).ToList().Select(s =>
                    {
                        var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
                        return CreateUserApi(s, passport);
                    });

                    break;
                case "Email":

                    return dbContext.Users.Where(s => s.Email.ToLower().Contains(text)).ToList().Select(s =>
                    {
                        var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
                        return CreateUserApi(s, passport);
                    });

                    break;

                default:

                    return dbContext.Users.ToList().Select(s =>
                    {
                        var passport = dbContext.Passports.FirstOrDefault(p => p.Id == s.PassportId);
                        return CreateUserApi(s, passport);
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
