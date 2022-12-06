using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelsApi;
using MyCar.Server.DB;
using MyCar.Server.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MyCar.Server.Controllers.Jwt
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration, MyCar_DBContext dbContext)
        {
            _configuration = configuration;
            this.dbContext = dbContext;
        }

        private UserApi CreateUserApi(User user, Passport passport)
        {
            var clientApi = (UserApi)user;
            clientApi.Passport = (PassportApi)passport;
            return clientApi;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(string userName, string Password)
        {
            //Passport passport = (Passport)request.Passport;
            //await dbContext.Passports.AddAsync(passport);
            //await dbContext.SaveChangesAsync();
            User user = new User();
            //CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);
            //user.UserName = request.Username;
            user.UserName = userName;
            user.PasswordHash = passwordHash;
            user.SaltHash = passwordSalt;
            //user.PassportId = passport.Id;
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Ok(user); //user
        }

        //[HttpPost("login")]
        [HttpGet("UserName, Password")]
        public async Task<ActionResult<UserApi>> Login(string userName, string Password)//UserDto request
        {
            User user = new User();
            user.UserName = userName;

            if (!FindUser(userName))
            {
                return BadRequest("User Not Found!");
            }
            

            user = await dbContext.Users.FirstOrDefaultAsync(s => s.UserName == userName);

            if (user.PasswordHash == null || !VerifyPasswordHash(Password, user.PasswordHash, user.SaltHash))
            {
                return BadRequest("Wrong password!");
            }

            string token = CreateToken(user);

            Passport passport = await dbContext.Passports.FindAsync(user.PassportId);
            UserType userType = await dbContext.UserTypes.FindAsync(user.UserTypeId);
            var client = (UserApi)user;
            client.Passport = (PassportApi)passport;
            client.UserType = (UserTypeApi)userType;
            return client;
            //user or token
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                //new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            User user = new User();
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private bool FindUser(string username)
        {
            var result =  dbContext.Users.FirstOrDefault(s=> s.UserName == username);
            if (result == null)
            {
                return false;
            }
            return true;
        }

    }

}
