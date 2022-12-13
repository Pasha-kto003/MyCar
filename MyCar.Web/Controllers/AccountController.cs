using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DTO;
using MyCar.Web.Core;
using MyCar.Web.Models;
using System.Security.Claims;

namespace MyCar.Web.Controllers
{
    public class AccountController : Controller
    {
        public List<UserApi> Users { get; set; } = new List<UserApi>();
        public int UserId = -1;
        public UserApi Userex { get; set; }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                Userex = await Api.Enter<UserApi>(model.UserName, model.Password, "Auth");

                if (User != null)
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
        public async Task<IActionResult> Personal_Area()
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
            var name = User.Identity.Name;
            if (name != null)
            {
                var user = Users.FirstOrDefault(s => s.UserName == name);
                var role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
                ViewData["content"] = $"Теперь ваша роль {role}";
                return View(user);
            }

            return NotFound();
        }
    }
}
