using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Web.Core;
using System.Linq.Expressions;

namespace MyCar.Web.Controllers
{
    public class UserController : Controller
    {

        public List<UserApi> Users { get; set; } = new List<UserApi>();
        public async Task<IActionResult> UpdateMethod(UserApi newUser)
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
            var user = Users.FirstOrDefault(s => s.ID == newUser.ID);
            if (user != null)
            {
                user.UserName = newUser.UserName;
                user.Email = newUser.Email;
                UserEdit(user);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
            

        }

        public async Task UserEdit(UserApi userApi)
        {
            var user = await Api.PutAsync<UserApi>(userApi, "User");
        }

        public IActionResult Index()
        {
            return View();
        }


    }
}
