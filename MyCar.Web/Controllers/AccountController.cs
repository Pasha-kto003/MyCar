﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;
using MyCar.Server.DTO;
using MyCar.Web.Core;
using MyCar.Web.Core.Hash;
using MyCar.Web.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MyCar.Web.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<UserApi> userManager;

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

        public IActionResult EditPasswordView()
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
                user.Passport.Number = newUser.Passport.Number;
                user.Passport.DateEnd = newUser.Passport.DateEnd;
                user.Passport.DateStart = newUser.Passport.DateStart;
                var errorUser = Users.FirstOrDefault(s => s.ID != user.ID && s.UserName == user.UserName);
                if (errorUser != null)
                {
                    TempData["SameUserError"] = $"Пользователь с никнеймом {errorUser.UserName} уже существует";
                    return View("Personal_Area", user);
                }

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


        [HttpPost]
        public async Task<IActionResult> EditPassword(EditPasswordModel registerModel)
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
            var user = Users.LastOrDefault(s => s.UserName == registerModel.UserName);
            if (ModelState.IsValid)
            {
                HashCheck.CreatePasswordHash(registerModel.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.SaltHash = passwordSalt;

                await ChangeUser(user);
                return RedirectToAction("Index", "Home");
            }
            if (user == null)
            {
                ModelState.AddModelError("", "Такого пользователя не существует");
                return View("EditPasswordView", registerModel);
            }
            if (user != null && user.UserType.TypeName == "Администратор")
            {
                ModelState.AddModelError("", "Нельзя редактировать этого пользователя");
                return View("EditPasswordView", registerModel);
            }
            if (user != null && user.UserType.TypeName == "Сотрудник")
            {
                ModelState.AddModelError("", "Нельзя редактировать этого пользователя");
                return View("EditPasswordView", registerModel);
            }
            else if (registerModel.UserName == "" || registerModel.UserName == null)
            {
                ModelState.TryAddModelError("", "Не введен логин");
                return View("EditPasswordView", registerModel);
            }
            else if (registerModel.Password == "" || registerModel.Password == null)
            {
                ModelState.TryAddModelError("", "Не введен пароль");
                return View("EditPasswordView", registerModel);
            }
            else if (registerModel.ConfirmPassword == "" || registerModel.ConfirmPassword == null)
            {
                ModelState.TryAddModelError("", "Повторите свой пароль");
                return View("EditPasswordView", registerModel);
            }
            else if (registerModel.ConfirmPassword != registerModel.Password)
            {
                ModelState.TryAddModelError("", "Неверный повтор пароля");
                return View("EditPasswordView", registerModel);
            }

            return View("EditPasswordView", registerModel);
        }

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
                    if (Userex.UserType.TypeName == "Администратор")
                    {
                        TempData["AllertMessage"] = "You log in as admin!!!";

                        return RedirectToAction("Index", "Home");
                    }
                    if (Userex.UserType.TypeName == "Клиент")
                    {
                        TempData["ClientMessage"] = "You log in as client!!!";

                        return RedirectToAction("Index", "Home");
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Такого пользователя нет на сайте");

            }
            else if (model.UserName == "" || model.UserName == null)
            {
                ModelState.TryAddModelError("", "Не введен логин");
                return View(model);
            }
            else if (model.Password == "" || model.Password == null)
            {
                ModelState.TryAddModelError("", "Не введен пароль");
                return View(model);
            }
            else
            {
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
            EmailIsValid(model.Email);
            if (EmailIsValid(model.Email) == false)
            {
                ModelState.AddModelError("", "Данные почты введены неккоректно");
                return View(model);
            }
            if(model.Password == null || model.Password == "")
            {
                ModelState.AddModelError("", "Пароль не введен");
                return View(model);
            }
            if (model.ConfirmPassword.ToString() != model.Password.ToString())
            {
                ModelState.AddModelError("", $"Пароли не совпадают");
                return View(model);
            }
            var user = Users.FirstOrDefault(s=> s.UserName == model.UserName || s.Email == model.Email);
            if (user != null)
            {
                ModelState.AddModelError("", $"Пользователь с именем {model.UserName} уже существует");
                return View(model);
            }
            
            await CreateUser(model);
            if (UserId != -1)
            {
                await GetUser(UserId);

                await Authenticate(Userex);

                if (Userex.UserType.TypeName == "Администратор")
                {
                    TempData["RegisterMessage"] = "Вы успешно зарестрировались на сайте";

                    return RedirectToAction("Index", "Home");
                }

                if (Userex.UserType.TypeName == "Клиент")
                {
                    TempData["RegisterMessage"] = "Вы успешно зарестрировались на сайте как клиент";

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
            UserDto userDto = new UserDto { Password = model.Password, Username = model.UserName, Email = model.Email };
            UserId = await Api.RegistrationAsync<UserDto>(userDto, "Auth");
        }

        public async Task GetUser(int id)
        {
            Userex = await Api.GetAsync<UserApi>(id, "User");
        }

        private async Task Authenticate(UserApi user)
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

        private async Task ChangeUser(UserApi userApi)
        {
            var user = await Api.PutAsync<UserApi>(userApi, "User");
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

        [Authorize(Roles = "Администратор, Клиент")]
        public async Task<IActionResult> PersonalUser(int id)
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");      
            var user = Users.FirstOrDefault(s => s.ID == id);
            var role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            ViewData["content"] = $"Теперь ваша роль {role}";
            return View("Personal_Area", user);

            //return NotFound();
        }

        public bool EmailIsValid(string email)
        {
            //string expression = "\\w+([-+.']\\w+)@\\w+([-.]\\w+)\\.\\w+([-.]\\w+)*";
            string expression = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            //"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$"
            //@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$"
            //@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z"

            if (Regex.IsMatch(email, expression))
            {
                if (Regex.Replace(email, expression, string.Empty).Length == 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}