using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Web.Core;
using MyCar.Web.Models;
using SmartBreadcrumbs.Attributes;
using System.Diagnostics;
using System.Security.Claims;

namespace MyCar.Web.Controllers
{
    [DefaultBreadcrumb]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = "Администратор, Клиент")]
        public async Task<IActionResult> Index()
        {
            var marks = new List<MarkCarApi>();
            marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            return View(marks);
        }

        [Breadcrumb(FromAction = "Index", Title = "Marks")]
        public async Task<IActionResult> MarkView()
        {
            var marks = new List<MarkCarApi>();
            marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            return View("MarkView", marks);
        }

        [Breadcrumb(FromAction = "Index", Title = "Users")]
        public async Task<IActionResult> UserView()
        {
            var users = new List<UserApi>();
            users = await Api.GetListAsync<List<UserApi>>("User");
            return View("UserView", users);
        }

        [Breadcrumb(FromAction = "Index", Title = "Auto")]
        [HttpGet]
        public async Task<IActionResult> GetAuto(int id)
        {
            var marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            var mark = marks.FirstOrDefault(s => s.ID == id);
            var cars = await Api.GetListAsync<List<CarApi>>("Car");
            cars = cars.Where(s => s.Model.MarkId == mark.ID).ToList();
            return View("CarView", cars);
        }

        [Breadcrumb(FromAction = "Index", Title = "CarView")]
        public async Task<IActionResult> CarView()
        {
            var cars = new List<CarApi>();
            cars = await Api.GetListAsync<List<CarApi>>("Car");
            return View("CarView", cars);
        }

        public IActionResult SpecialCarView()
        {
            return View();
        }
        [Breadcrumb("ViewData.Title")]
        public IActionResult Privacy()
        {
            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}