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

        public List<SaleCarApi> Cars { get; set; } = new List<SaleCarApi>();
        public List<SaleCarApi> FullCars { get; set; }
        public List<MarkCarApi> Marks { get; set; }

        public List<CarApi> NewCars { get; set; } = new List<CarApi>();
        public List<CarApi> NewFullCars { get; set; } = new List<CarApi>();

        public List<DiscountApi> Discounts { get; set; } = new List<DiscountApi>();

        public List<ActionTypeApi> Types { get; set; }
        public List<StatusApi> Statuses { get; set; }
        public List<WareHouseApi> Warehouses { get; set; } = new List<WareHouseApi>();
        public List<OrderApi> Orders = new List<OrderApi>();
        public List<UserApi> Users { get; set; } = new List<UserApi>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("Home/ShowPartialView/id/{id?}")]
        [Breadcrumb("ShowPartialView", FromController = typeof(HomeController), FromAction = "CarView")]
        public async Task<IActionResult> ShowPartialView(int id)
        {
            NewCars = GetCar().Result;
            Marks = GetMark().Result;
            var car = NewCars.FirstOrDefault(s=> s.ID == id);
            var page = new MvcBreadcrumbNode("Index", "Home", "Главная страница");
            var articlesPage = new MvcBreadcrumbNode("CarView", "Home", "Список авто") { Parent = page };
            var articlePage = new MvcBreadcrumbNode("ShowPartialView", "Home", $"Комплектации / {car.CarName}") { Parent = articlesPage };
            RouteAttribute route = new RouteAttribute($"/Car/DetailsCarView/CarName/{car.CarName}");
            ViewData["BreadcrumbNode"] = articlePage;
            ViewBag.MarkCars = Marks;
            return View("ChooseCarView", car);
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
            NewCars = GetCar().Result;
            var page = new MvcBreadcrumbNode("Index", "Home", "Главная страница");
            var articlesPage = new MvcBreadcrumbNode("CarView", "Home", "Список авто") { Parent = page };
            ViewData["BreadcrumbNode"] = articlesPage;
            var marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            var mark = marks.FirstOrDefault(s => s.ID == id);
            string text = mark.MarkName;
            string type = "Марка";
            string filter = "Все";
            var cars = NewCars;
            NewCars = NewCars.Where(s=> s.CarMark == text).ToList();
            List<MarkCarApi> markCars;
            markCars = GetMark().Result;
            ViewBag.MarkCars = markCars;
            return View("CarView", NewCars);
        }

        

        #region To Car Pages
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

        public async Task<IActionResult> ElectricCarView()
        {
            var cars = await Api.GetListAsync<List<CarApi>>("Car");
            return View("ElectricCarView", cars);
        }
        #endregion

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
        public async Task<IActionResult> CarView()
        {
            var page = new MvcBreadcrumbNode("Index", "Home", "Главная страница");
            var articlesPage = new MvcBreadcrumbNode("CarView", "Home", "Список авто") { Parent = page };
            ViewData["BreadcrumbNode"] = articlesPage;
            await GetWare();
            NewCars = GetCar().Result;
            Marks = GetMark().Result;
            ViewBag.MarkCars = Marks;
            var fullCars = NewCars.DistinctBy(s => s.CarName);
            return View("CarView", fullCars);
        }

        [Breadcrumb(FromAction = "Index", Title = "Список авто")]
        [HttpGet]
        public async Task<IActionResult> DiscountCarView()
        {
            DateTime dateTime = DateTime.Now;
            var page = new MvcBreadcrumbNode("Index", "Home", "Главная страница");
            var articlesPage = new MvcBreadcrumbNode("DiscountCarView", "Home", "Список скидочных авто") { Parent = page };
            ViewData["BreadcrumbNode"] = articlesPage;
            await GetWare();
            NewCars = GetCar().Result;
            Marks = GetMark().Result;
            ViewBag.MarkCars = Marks;

            var discounts = GetDiscount().Result.Where(s => s.SaleCarId != 0 && s.EndDate >= dateTime && s.StartDate <= dateTime).Select(s=> s.SaleCar.Car).ToList();
            if (discounts.Any())
            {
                ViewBag.CatalogCar = "Список скидочных авто";
                return View("CarView", discounts);
            }
            ViewBag.CatalogCar = "Каталог авто";
            return View("DiscountCarView", NewCars);
        }

        public async Task<IActionResult> Sort(string SortString, string Filterstring, string searchText)
        {
            List<CarApi> cars;
            List<MarkCarApi> markCars;
            cars = GetCar().Result;
            markCars = GetMark().Result;
            ViewBag.MarkCars = markCars;
            string filter = "Все";
            if (SortString == "По умолчанию")
            {
                cars = await Api.SearchFilterAsync<List<CarApi>>(Filterstring, searchText, "Car", filter);
                NewFullCars = cars.DistinctBy(s => s.CarName).ToList();
                return View("CarView", NewFullCars);
            }
            else if (SortString == "По возрастанию")
            {
                cars = await Api.SearchFilterAsync<List<CarApi>>(Filterstring, searchText, "Car", filter);
                cars.Sort((x, y) => x.CarPrice.Value.CompareTo(y.CarPrice));
                NewFullCars = cars.DistinctBy(s => s.CarName).ToList();
                return View("CarView", NewFullCars);
            }
            else if (SortString == "По убыванию")
            {
                cars = await Api.SearchFilterAsync<List<CarApi>>(Filterstring, searchText, "Car", filter);
                cars.Sort((x, y) => y.CarPrice.Value.CompareTo(x.CarPrice));
                NewFullCars = cars.DistinctBy(s => s.CarName).ToList();
                return View("CarView", NewFullCars);

            }

            return View("CarView", NewFullCars);
        }

        public async Task<IActionResult> SearchCar(string? Filterstring, string? SearchString, string? SearchPrice, string? SortString)
        {
            List<CarApi> cars;
            List<MarkCarApi> markCars;
            cars = GetCar().Result;
            markCars = GetMark().Result;
            ViewBag.MarkCars = markCars;
            string? type = "Авто";
            string? filter = "Все";
            string? searchText = SearchString?.ToLower() ?? "";
            decimal? price = decimal.Parse(SearchPrice);
            if (searchText != "" && Filterstring != "Тип поиска" && SortString != "" && SortString != null)
            {
                cars = await Api.SearchFilterAsync<List<CarApi>>(Filterstring, searchText, "Car", filter);
                NewFullCars = cars.Where(s => s.CarPrice >= price).DistinctBy(s => s.CarName).ToList();
                await Sort(SortString, Filterstring, searchText);
                TempData["SearchMessage"] = $"По вашему запросу найдено {NewFullCars.Count} авто";
                return View("CarView", NewFullCars);
            }
            if (searchText == "" || Filterstring == "Тип поиска" || SearchPrice != "" || filter == "Все" || SortString != "" || SortString != null)
            {
                NewFullCars = cars.Where(s=> s.CarPrice >= price).DistinctBy(s => s.CarName).ToList();
                await Sort(SortString, Filterstring, searchText);
                return View("CarView", NewFullCars);
            }
            if(SearchPrice != "")
            {
                NewFullCars = cars.Where(s => s.CarPrice >= price).DistinctBy(s => s.CarName).ToList();
                return View("CarView", NewFullCars);
            }
            else
            {
                NewFullCars = cars.DistinctBy(s => s.CarName).ToList();
                return View("CarView", NewFullCars);
            }
        }

        public async Task<IActionResult> SearchDiscountCar(string? Filterstring, string? SearchString, string? SearchPrice, string? SortString)
        {
            List<CarApi> cars;
            List<MarkCarApi> markCars;
            cars = GetCar().Result;
            markCars = GetMark().Result;
            ViewBag.MarkCars = markCars;
            string type = "Авто";
            string filter = "Все";
            string? searchText = SearchString?.ToLower() ?? "";
            decimal? price = decimal.Parse(SearchPrice);
            if (searchText != "" && Filterstring != "Тип поиска" && SortString != "" && SortString != null)
            {
                cars = await Api.SearchFilterAsync<List<CarApi>>(Filterstring, searchText, "Car", filter);
                NewFullCars = cars.Where(s => s.CarPrice >= price).DistinctBy(s => s.CarName).ToList();
                await Sort(SortString, Filterstring, searchText);
                TempData["SearchMessage"] = $"По вашему запросу найдено {NewFullCars.Count} авто";
                return View("DiscountCarView", NewFullCars);
            }
            if (searchText == "" || Filterstring == "Тип поиска" || filter == "Все" || SortString != "" || SortString != null)
            {
                NewFullCars = cars.Where(s => s.CarPrice >= price).DistinctBy(s => s.CarName).ToList();
                await Sort(SortString, Filterstring, searchText);
                return View("DiscountCarView", NewFullCars);
            }
            if (SearchPrice != "")
            {
                NewFullCars = cars.Where(s => s.CarPrice >= price).DistinctBy(s => s.CarName).ToList();
                return View("DiscountCarView", NewFullCars);
            }
            else
            {
                NewFullCars = cars.DistinctBy(s => s.CarName).ToList();
                return View("DiscountCarView", NewFullCars);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchLexus()
        {
            var models = await Api.GetListAsync<List<ModelApi>>("Model");
            var text = "RC F";
            var type = "Модель";
            var model = models.FirstOrDefault(s => s.ModelName.Contains("RC F"));
            var cars = await Api.GetListAsync<List<CarApi>>("Car");
            string filter = "Все";
            cars = await Api.SearchFilterAsync<List<CarApi>>(text, type, "Car", filter);
            var markCars = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            ViewBag.MarkCars = markCars;
            return View("CarView", cars);
        }

        public async Task<IActionResult> SetListFilter()
        {
            return View("CarView");
        }

        private async Task<List<CarApi>> GetCar()
        {
            NewCars = await Api.GetListAsync<List<CarApi>>("Car");
            return NewCars;
        }

        private async Task<List<SaleCarApi>> GetSale()
        {
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
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

        public async Task<List<DiscountApi>> GetDiscount()
        {
            Discounts = await Api.GetListAsync<List<DiscountApi>>("Discount");
            return Discounts;
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