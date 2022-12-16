using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Web.Core;
using MyCar.Web.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MyCar.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = "Администратор, Клиент")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserView()
        {
            var users = new List<UserApi>();
            users = await Api.GetListAsync<List<UserApi>>("User");
            return View("UserView", users);
        }

        public async Task<IActionResult> CarView()
        {
            var cars = new List<CarApi>();
            cars = await Api.GetListAsync<List<CarApi>>("Car");
            foreach(CarApi car in cars)
            {
                car.PhotoCar = $@"C:/Users/User/source/repos/MyCar/MyCar.Web/bin/Debug/net6.0/CarImages/{car.PhotoCar}";
            }
            return View("CarView", cars);
        }

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