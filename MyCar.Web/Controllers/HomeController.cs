using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using ModelsApi;
using MyCar.Server.DB;
using MyCar.Web.Core;
using MyCar.Web.Core.Paging;
using MyCar.Web.Models;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using System.Diagnostics;
using System.Security.Claims;

namespace MyCar.Web.Controllers
{
    [DefaultBreadcrumb]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public List<SaleCarApi> Cars { get; set; }
        public List<SaleCarApi> FullCars { get; set; }
        public List<MarkCarApi> Marks { get; set; }

        public List<ActionTypeApi> Types { get; set; }
        public List<StatusApi> Statuses { get; set; }
        public List<WareHouseApi> Warehouses { get; set; } = new List<WareHouseApi>();
        public List<OrderApi> Orders = new List<OrderApi>();
        public List<UserApi> Users { get; set; } = new List<UserApi>();

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
        public async Task<IActionResult> DetailsCar(int id)
        {
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            var car = cars.FirstOrDefault(s => s.ID == id);
            var marks = new List<MarkCarApi>();
            marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            if (id == 0)
            {
                ViewBag.Marks = marks;
                return View("Index", cars);
            }
            ViewBag.Marks = marks;
            ViewBag.ID = car.ID;
            ViewBag.CarName = car.FullName;
            ViewBag.FullPrice = car.FullPrice;
            ViewBag.PhotoCar = car.Car.PhotoCar;
            return View("Index", cars);
        }

        [Breadcrumb(FromAction = "Index", Title = "Марки авто")]
        public async Task<IActionResult> MarkView()
        {
            var page = new MvcBreadcrumbNode("Index", "Home", "Главная страница");
            var articlesPage = new MvcBreadcrumbNode("MarkView", "Home", "Список марок авто") { Parent = page };
            ViewData["BreadcrumbNode"] = articlesPage;
            var marks = new List<MarkCarApi>();
            marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            return View("MarkView", marks);
        }

        [Breadcrumb(FromAction = "Index", Title = "Пользователи")]
        public async Task<IActionResult> UserView()
        {
            var page = new MvcBreadcrumbNode("Index", "Home", "Главная страница");
            var articlesPage = new MvcBreadcrumbNode("UserView", "Home", "Список пользователей") { Parent = page };
            ViewData["BreadcrumbNode"] = articlesPage;
            var users = new List<UserApi>();
            users = await Api.GetListAsync<List<UserApi>>("User");
            return View("UserView", users);
        }

        public async Task<IActionResult> DashBoardView()
        {
            var page = new MvcBreadcrumbNode("Index", "Home", "Главная страница");
            var articlesPage = new MvcBreadcrumbNode("DashBoardView", "Home", "Статистика") { Parent = page };
            ViewData["BreadcrumbNode"] = articlesPage;
            await GetOrders();
            var price = Orders.Where(s => s.ActionType.ActionTypeName == "Продажа").SelectMany(s => s.WareHouses).Sum(s => s.Price);
            var ware = Orders.Where(s => s.ActionType.ActionTypeName == "Поступление").SelectMany(s => s.WareHouses).Sum(s=> s.Price);
            ViewBag.SalePrice = price;
            ViewBag.WareHousePrice = ware;
            var saleCount = Orders.Where(s => s.ActionType.ActionTypeName == "Продажа").SelectMany(s => s.WareHouses).Count();
            ViewBag.SaleCount = saleCount;
            var wareCount = Orders.Where(s => s.ActionType.ActionTypeName == "Поступление").SelectMany(s => s.WareHouses).Count();
            ViewBag.WareHouseCount = wareCount;
            ViewBag.SaleCarCount = Orders.Where(s => s.ActionType.ActionTypeName == "Продажа").SelectMany(s => s.WareHouses).Count();
            return View("DashBoardView");
        }

        [Breadcrumb(FromAction = "Index", Title = "Список авто")]
        [HttpGet]
        public async Task<IActionResult> GetAuto(int id)
        {
            var page = new MvcBreadcrumbNode("Index", "Home", "Главная страница");
            var articlesPage = new MvcBreadcrumbNode("CarView", "Home", "Список авто") { Parent = page };
            ViewData["BreadcrumbNode"] = articlesPage;
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

        public async Task<IActionResult> ElectricCarView()
        {
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            return View("ElectricCarView", cars);
        }

        public async Task<IActionResult> LexusGXView()
        {
            return View("LexusGXView");
        }

        public async Task<IActionResult> FutureCars()
        {
            return View("FutureCarView");
        }

        public async Task<IActionResult> ToyotaCamryView()
        {
            return View("ToyotaCamryView");
        }

        public async Task<IActionResult> LexusRCFView()
        {
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            ViewBag.Cars = cars.Where(s=> s.Car.CarName.Contains("Lexus RCF"));
            return View("LexusRCFView");
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

        [Breadcrumb(FromAction = "Index", Title = "Список авто")]
        [HttpGet]
        public async Task<IActionResult> CarView()//int? pageNumber
        {
            var page = new MvcBreadcrumbNode("Index", "Home", "Главная страница");
            var articlesPage = new MvcBreadcrumbNode("CarView", "Home", "Список авто") { Parent = page };
            ViewData["BreadcrumbNode"] = articlesPage;
            await GetWare();
            List<SaleCarApi> cars;
            List<MarkCarApi> markCars;
            cars = GetCar().Result;
            markCars = GetMark().Result;
            ViewBag.MarkCars = markCars;
            var fullCars = cars.DistinctBy(s => s.FullName);
            return View("CarView", fullCars);
        }

        [Breadcrumb(FromAction = "Index", Title = "Список авто")]
        [HttpGet]
        public async Task<IActionResult> SearchCar(string SearchString, string FilterString, string SortString)
        {
            List<SaleCarApi> cars;
            List<MarkCarApi> markCars;
            cars = GetCar().Result;
            markCars = GetMark().Result;
            ViewBag.MarkCars = markCars;
            string type = "Авто";
            string filter = "Все";
            if (SortString == "По умолчанию")
            {
                FullCars = cars;
            }
            else if (SortString == "По возрастанию")
            {
                cars.Sort((x, y) => x.FullPrice.Value.CompareTo(y.FullPrice));
                FullCars = cars;
            }
            else if (SortString == "По убыванию")
            {
                cars.Sort((x, y) => y.FullPrice.Value.CompareTo(x.FullPrice));
                FullCars = cars;
            }
            if(SearchString != null || FilterString != "Тип поиска" || filter != "Все")
            {
                FullCars = await Api.SearchFilterAsync<List<SaleCarApi>>(FilterString, SearchString, "CarSales", filter);
                //FullCars = cars.Where(s => s.FullName == SearchString).ToList();
            }
            return View("CarView", FullCars);
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

        public async Task<IActionResult> SetListFilter()
        {
            return View("CarView");
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

        private async Task GetWare()
        {
            Warehouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
        }

        public async Task GetOrders()
        {
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            Warehouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
            Users = await Api.GetListAsync<List<UserApi>>("User");
            Statuses = await Api.GetListAsync<List<StatusApi>>("Status");
            Types = await Api.GetListAsync<List<ActionTypeApi>>("ActionType");
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
        }
    }
}