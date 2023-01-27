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
        public async Task<IActionResult> DetailsCarView(string CarName)
        {
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            var car = Cars.FirstOrDefault(s => s.Car.CarName == CarName);
            var page = new MvcBreadcrumbNode("Index", "Home", "Home");
            var articlesPage = new MvcBreadcrumbNode("CarView", "Home", "CarView") { Parent = page };
            var articlePage = new MvcBreadcrumbNode("DetailsCarView", "Car", $"DetailsCarView / {CarName}") { Parent = articlesPage };
            ViewData["BreadcrumbNode"] = articlePage;
            ViewData["Title"] = $"CarName - {CarName}";
            ViewBag.SaleCars = Cars.Where(s=> s.Car.CarMark.Contains(car.Car.CarMark));
            var carModel = new CarModel() { CarName = car.Car.CarName };
            return View(car);
        }

        

        [HttpGet]
        public IActionResult CartView()
        {
            return View();
        }
    }
}
