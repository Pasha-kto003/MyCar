using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;
using MyCar.Server.DTO;
using MyCar.Web.Core;
using MyCar.Web.Core.Hash;
using MyCar.Web.Models;
using System.Security.Claims;

namespace MyCar.Web.Controllers
{
    public class AccountController : Controller
    {
        public List<UserApi> Users { get; set; } = new List<UserApi>();
        public int UserId = -1;
        public UserApi Userex { get; set; }
        //public User UserModel { get; set; }
        // GET: AccountController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(int id)
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
            var user = Users.FirstOrDefault(s => s.ID == id);
            return View(user);
        }



        [Authorize(Roles = "Администратор, Клиент")]
        public async Task<IActionResult> PersonalArea()
        {
            var uname = User.Identity.Name;
            if (uname != null)
            {
                Users = await Api.GetListAsync<List<UserApi>>("User");
                var user = Users.FirstOrDefault(s => s.UserName == uname);
                string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
                ViewData["Content"] = $"Теперь ваша роль {role}";
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditUserView(int id)
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
            var user = Users.FirstOrDefault(s => s.ID == id);
            //UserEdit(user);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return NotFound();
            }
        }

        //public async Task UserEdit(UserApi userApi)
        //{
        //    var user = await Api.PutAsync<UserApi>(userApi, "User");
        //}
        #region updating
        public async Task<IActionResult> UpdateMethod(UserApi newUser)
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
            var user = Users.FirstOrDefault(s => s.ID == newUser.ID);
            if (user != null)
            {
                user.UserName = newUser.UserName;
                user.Email = newUser.Email;
                user.Passport.FirstName = newUser.Passport.FirstName;
                user.Passport.LastName = newUser.Passport.LastName;
                user.Passport.Patronimyc = newUser.Passport.Patronimyc;
                user.Passport.DateStart = newUser.Passport.DateStart;
                user.Passport.DateEnd = newUser.Passport.DateEnd;
                UserEdit(user, user.Passport);
                Authenticate(user);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return BadRequest();
            }
        }
        public async Task UserEdit(UserApi userApi, PassportApi passportapi)
        {
            var user = await Api.PutAsync<UserApi>(userApi, "User");
            var passport = await Api.PutAsync<PassportApi>(passportapi, "Passport");
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                Userex = await Api.Enter<UserApi>(model.UserName, model.Password, "Auth");

                if (Userex != null)
                {
                    Authenticate(Userex);
                    if(Userex.UserType.TypeName == "Администратор")
                    {
                        TempData["AllertMessage"] = "You log in as admin!!!";

                        return RedirectToAction("Index", "Home");
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");

            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            await CreateUser(model);
            if (UserId != -1)
            {
                await GetUser(UserId);

                await Authenticate(Userex);

                if (Userex.UserType.TypeName == "admin")
                {
                    TempData["AllertMessage"] = "You log in as admin!!!";

                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Index", "Home");

            }
            else
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            
            return View(model);
        }

        //[HttpGet]
        //public async Task<IActionResult> EditUserView(int id)
        //{
        //    Users = await Api.GetListAsync<List<UserApi>>("User");
        //    if (id != 0)
        //    {
        //        var user = Users.FirstOrDefault(s => s.ID == id);
        //        return View(user);
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}

        private async Task ChangeUser(UserApi userApi, PassportApi passportapi)
        {
            var user = await Api.PutAsync<UserApi>(userApi, "User");
            var passport = await Api.PutAsync<PassportApi>(passportapi, "Passport");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login", "Account");
        }

        private async Task CreateUser(RegisterModel model) 
        {
            UserDto userDto = new UserDto { Password = model.Password, Username = model.UserName };
            UserId = await Api.RegistrationAsync<UserDto>(userDto, "Auth");
        }

        public async Task GetUser(int id)
        {
            Userex = await Api.GetAsync<UserApi>(id, "User");
        }

        private async Task Authenticate(UserApi user) //
        {
            
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserType?.TypeName)
            };
           
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }



        [Authorize(Roles = "Администратор, Клиент")]
        public async Task<IActionResult> Personal_Area(string name)
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
            name = User.Identity.Name;
            var user = Users.FirstOrDefault(s => s.UserName == name);
            var role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            ViewData["content"] = $"Теперь ваша роль {role}";
            return View(user);

            //return NotFound();
        }
    }
}
