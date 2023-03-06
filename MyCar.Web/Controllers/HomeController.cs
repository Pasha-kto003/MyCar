using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using ModelsApi;
using MyCar.Server.DB;
using MyCar.Web.Core;
using MyCar.Web.Core.Paging;
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


        [Authorize(Roles = "Администратор, Клиент")]
        public async Task<IActionResult> Index()
        {
            var marks = new List<MarkCarApi>();          
            marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            ViewBag.Marks = marks;
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            return View("Index", cars);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsCar(string CarName)
        {
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            var car = cars.FirstOrDefault(s=> s.Car.CarName == CarName);
            var marks = new List<MarkCarApi>();
            marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            ViewBag.Marks = marks;
            ViewBag.CarName = car.Car.CarName;
            ViewBag.FullPrice = car.FullPrice;
            ViewBag.PhotoCar = car.Car.PhotoCar;
            return View("Index", cars);
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

        [Breadcrumb(FromAction = "Index", Title = "CarView")]
        [HttpGet]
        public async Task<IActionResult> GetAuto(int id)
        {
            var marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            var mark = marks.FirstOrDefault(s => s.ID == id);
            string text = mark.MarkName;
            string type = "Марка";
            string filter = "Все";
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            cars = await Api.SearchFilterAsync<List<SaleCarApi>>(text, type, "CarSales", filter);
            List<MarkCarApi> markCars;
            markCars = GetMark().Result;
            ViewBag.MarkCars = markCars;
            return View("CarView", cars);
        }

        //[Breadcrumb(FromAction = "Index", Title = "CarView")]
        //public async Task<IActionResult> CarView()
        //{
        //    var cars = new List<SaleCarApi>();
        //    cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
        //    return View("CarView", cars);
        //}

        public async Task<IActionResult> ElectricCarView()
        {
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            return View("ElectricCarView", cars);
        }

        public async Task<IActionResult> LexusGXView()
        {
            return View("LexusGXView");
        }

        public async Task<IActionResult> LexusRCFView()
        {
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            ViewBag.Cars = cars.Where(s=> s.Car.CarName.Contains("Lexus RCF"));
            return View("LexusRCFView");
        }

        public async Task<IActionResult> SearchLexus()
        {
            var models = await Api.GetListAsync<List<ModelApi>>("Model");
            var text = "RC F";
            var type = "Модель";
            var model = models.FirstOrDefault(s => s.ModelName.Contains("RC F"));
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            string filter = "Все";
            cars = await Api.SearchFilterAsync<List<SaleCarApi>>(text, type, "CarSales", filter);
            List<MarkCarApi> markCars;
            markCars = GetMark().Result;
            ViewBag.MarkCars = markCars;
            return View("CarView", cars);
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
        public async Task<IActionResult> CarView(string SearchString)//int? pageNumber
        {

            List<SaleCarApi> cars;
            List<MarkCarApi> markCars;
            cars = GetCar().Result;
            markCars = GetMark().Result;
            ViewBag.MarkCars = markCars;
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

            //if (SearchString != null)
            //{
            //    pageNumber = 1;
            //}
            //else
            //{
            //    SearchString = filter;
            //}
            //int pageSize = 3;
            return View("CarView", cars);
            //return View("CarView", await PaginatedList<SaleCarApi>.CreateAsync(cars, pageNumber ?? 1, pageSize));
        }

        private async Task<List<SaleCarApi>> GetCar()
        {
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            return Cars;
        }
        private async Task<List<MarkCarApi>> GetMark()
        {
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            return Marks;
        }
    }
}