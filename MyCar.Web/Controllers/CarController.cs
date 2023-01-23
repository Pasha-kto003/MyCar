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

        public List<CarApi> Cars { get; set; } = new List<CarApi>();
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
            Cars = await Api.GetListAsync<List<CarApi>>("Car");
            var car = Cars.FirstOrDefault(s => s.CarName == CarName);
            var page = new MvcBreadcrumbNode("Index", "Home", "Home");
            var articlesPage = new MvcBreadcrumbNode("CarView", "Home", "CarView") { Parent = page };
            var articlePage = new MvcBreadcrumbNode("DetailsCarView", "Car", $"DetailsCarView / {CarName}") { Parent = articlesPage };
            ViewData["BreadcrumbNode"] = articlePage;
            ViewData["Title"] = $"CarName - {CarName}";
            ViewBag.Cars = Cars.Where(s=> s.CarMark.Contains(car.CarMark));
            var carModel = new CarModel() { CarName = car.CarName };
            return View(car);
        }

        // GET: CarController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CarController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CarController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CarController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CarController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CarController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
