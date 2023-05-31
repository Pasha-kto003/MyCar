using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Web.Core;
using MyCar.Web.Models;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using MyCar.Web.Core;

namespace MyCar.Web.Controllers
{
    [DefaultBreadcrumb]
    public class CarController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public CarController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public List<SaleCarApi> Cars { get; set; } = new List<SaleCarApi>();
        public List<CarApi> NewCars { get; set; } = new List<CarApi>();
        public List<MarkCarApi> Marks { get; set; } = new List<MarkCarApi>();

        // GET: CarController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CarController/Details/5
        [Route("/Car/DetailsCarView/id/{id?}")]
        [Breadcrumb("DetailsCarView", FromController = typeof(HomeController), FromAction = "CarView")]
        public async Task<IActionResult> DetailsCarView(int id)
        {
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            var car = Cars.FirstOrDefault(s => s.ID == id);
            var page = new MvcBreadcrumbNode("Index", "Home", "Главная страница");
            var articlesPage = new MvcBreadcrumbNode("CarView", "Home", "Список авто") { Parent = page };
            var article = new MvcBreadcrumbNode("ShowPartialView", "Home", $"{car.Car.CarName}") { Parent = articlesPage };
            var articlePage = new MvcBreadcrumbNode("DetailsCarView", "Car", $"{car.FullName}") { Parent = article };
            RouteAttribute route = new RouteAttribute($"/Car/DetailsCarView/CarName/{car.FullName}");
            ViewData["BreadcrumbNode"] = articlePage;
            ViewData["Title"] = $"CarName - {car.Car.CarName}";
            ViewBag.SaleCars = Cars.Where(s=> s.Car.CarMark.Contains(car.Car.CarMark));
            ViewBag.RecommendCars = Cars.Where(s => s.Car.CarMark.Contains(car.Car.CarMark) && s.ID != car.ID);
            ViewBag.Cars = Cars.Where(s=> s.Car.ModelId == car.Car.ModelId);
            //foreach(SaleCarApi carStyle in ViewBag.Cars)
            //{
            //    ViewBag.DiscountCarStylePrice = DiscountCounter.GetDiscount(carStyle);
            //    ViewBag.FullPrice = carStyle.FullPrice;
            //}
            ViewBag.DiscountPrice = DiscountCounter.GetDiscount(car);
            return View(car);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsCar(string CarName)
        {
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            var car = cars.FirstOrDefault(s => s.Car.CarName == CarName);
            var marks = new List<MarkCarApi>();
            marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            ViewBag.Marks = marks;
            ViewBag.CarName = car.Car.CarName;
            ViewBag.FullPrice = car.FullPrice;
            ViewBag.PhotoCar = car.Car.PhotoCar;
            ViewBag.SaleCars = cars.Where(s => s.Car.CarMark.Contains(car.Car.CarMark));
            return View("DetailsCarView", car);
        }

        private async Task<List<MarkCarApi>> GetMark()
        {
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            return Marks;
        }

        [HttpGet]
        public IActionResult CartView()
        {
            return View();
        }
    }
}
