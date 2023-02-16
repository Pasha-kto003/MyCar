using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;
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

        public List<SaleCarApi> Cars { get; set; }
        public List<MarkCarApi> Marks { get; set; }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public List<SaleCarApi> Cars = new List<SaleCarApi>();
        public List<MarkCarApi> Marks = new List<MarkCarApi>();

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
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            cars = cars.Where(s => s.Car.Model.MarkId == mark.ID).ToList();
            return View("CarView", cars);
        }

        //[Breadcrumb(FromAction = "Index", Title = "CarView")]
        //public async Task<IActionResult> CarView(string? Name)
        //{
        //    string name = Name ?? string.Empty;
        //    var cars = new List<SaleCarApi>();
        //    if (name == "" || name == null)
        //    {
        //        cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
        //    }
        //    else
        //    {
        //        string type = "Название";
        //        string filter = "Все";
        //        //cars = await Api.SearchFilterAsync<List<CarApi>>(type, name, "Car", filter);
        //        cars = cars.Where(s=> s.Car.CarName.Contains(name)).ToList();
        //    }
        //    return View("CarView", cars);
        //}

        public async Task<IActionResult> ElectricCarView()
        {
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            return View("ElectricCarView", cars);
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

        [Breadcrumb(FromAction = "Index", Title = "CarView")]
        [HttpGet]
        public async Task<IActionResult> CarView(string SearchString)
        {
            List<SaleCarApi> cars;
            cars = GetCar().Result;
            string type = "Авто";
            string filter = "Все";
            if (SearchString == null)
            {
                await GetCar();
            }
            else
            {
                cars = await Api.SearchFilterAsync<List<SaleCarApi>>(type, SearchString, "CarSales", filter);
            }
            return View("CarView", cars);
        }

        private async Task<List<SaleCarApi>> GetCar()
        {
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            return Cars;
        }
    }
}