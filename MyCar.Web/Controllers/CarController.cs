using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Web.Core;
using MyCar.Web.Models;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;

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
        // GET: CarController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CarController/Details/5
        [Route("/Car/DetailsCarView/CarName/{CarName?}")]
        [Breadcrumb("DetailsCarView", FromController = typeof(HomeController), FromAction = "CarView")]
        public async Task<IActionResult> DetailsCarView(int id)
        {
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            var car = Cars.FirstOrDefault(s => s.ID == id);
            var page = new MvcBreadcrumbNode("Index", "Home", "Home");
            var articlesPage = new MvcBreadcrumbNode("CarView", "Home", "CarView") { Parent = page };
            var articlePage = new MvcBreadcrumbNode("DetailsCarView", "Car", $"DetailsCarView / {car.Car.CarName}") { Parent = articlesPage };
            RouteAttribute route = new RouteAttribute($"/Car/DetailsCarView/CarName/{car.Car.CarName}");
            ViewData["BreadcrumbNode"] = articlePage;
            ViewData["Title"] = $"CarName - {car.Car.CarName}";
            ViewBag.SaleCars = Cars.Where(s=> s.Car.CarMark.Contains(car.Car.CarMark));
            ViewBag.Cars = Cars.Where(s=> s.Car.ModelId == car.Car.ModelId);
            var carModel = new CarModel() { CarName = car.Car.CarName };
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

        [HttpGet]
        public async Task<IActionResult> SearchLexus()
        {
            var models = await Api.GetListAsync<List<ModelApi>>("Model");
            var text = "RC F";
            var type = "Модель";
            var model = models.FirstOrDefault(s => s.ModelName.Contains("RC F"));
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            string filter = "Все";
            cars = await Api.SearchFilterAsync<List<SaleCarApi>>(text, type, "CarSales", filter);
            var markCars = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            ViewBag.MarkCars = markCars;
            return View("CarView", cars);
        }

        [HttpGet]
        public IActionResult CartView()
        {
            return View();
        }
    }
}
