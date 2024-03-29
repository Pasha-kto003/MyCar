using ChartJSCore.Helpers;
using ChartJSCore.Models;
using ChartJSCore.Plugins.Zoom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.OpenApi.Validations;
using ModelsApi;
using MyCar.Server.DB;
using MyCar.Web.Core;
using MyCar.Web.Core.Charts;
using MyCar.Web.Core.Paging;
using MyCar.Web.Models;
using Newtonsoft.Json;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using static NuGet.Packaging.PackagingConstants;

namespace MyCar.Web.Controllers
{
    [DefaultBreadcrumb]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public List<SaleCarApi> Cars { get; set; } = new List<SaleCarApi>();
        public List<SaleCarApi> FullCars { get; set; }
        public List<MarkCarApi> Marks { get; set; }

        public static List<OrderApi> Orders1 = new List<OrderApi>();
        public static List<CarApi> CarsChart = new List<CarApi>();
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
            var orderItems = new List<WareHouseApi>();
            string json = HttpContext.Session.GetString("OrderItem");
            if (json != null)
                orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(json) ?? new List<WareHouseApi>();

            ViewBag.CountCart = orderItems.Count;
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

        public async Task<IActionResult> DisplayImage(string photoName)
        {
            byte[] imageData = await Api.GetImage<byte[]>($"{photoName}", "Image");

            return File(imageData, "image/png");
        }

        public async Task<IActionResult> DashBoardView(DateTime dateCompare)
        {
            var page = new MvcBreadcrumbNode("Index", "Home", "Главная страница");
            var articlesPage = new MvcBreadcrumbNode("DashBoardView", "Home", "Статистика") { Parent = page };
            ViewData["BreadcrumbNode"] = articlesPage;
            await GetOrders();
            var price = Orders.Where(s => s.ActionType.ActionTypeName == "Продажа").SelectMany(s => s.WareHouses).Sum(s => s.Price);
            var ware = Orders.Where(s => s.ActionType.ActionTypeName == "Поступление").SelectMany(s => s.WareHouses).Sum(s => s.Price);
            ViewBag.SalePrice = price;
            ViewBag.WareHousePrice = ware;
            var saleCount = Orders.Where(s => s.ActionType.ActionTypeName == "Продажа").SelectMany(s => s.WareHouses).Count();
            ViewBag.SaleCount = saleCount;
            var wareCount = Orders.Where(s => s.ActionType.ActionTypeName == "Поступление").SelectMany(s => s.WareHouses).Count();
            ViewBag.WareHouseCount = wareCount;
            ViewBag.SaleCarCount = Orders.Where(s => s.ActionType.ActionTypeName == "Продажа").SelectMany(s => s.WareHouses).Count();

            Chart pieChart = GeneratePieChart();// создание круговой диаграммы
            ViewData["PieChart"] = pieChart;

            //Chart vertBarChart = GenerateVerticalBarChart();// создание вертикальной диаграммы
            //ViewData["VertBarChart"] = vertBarChart;

            //Chart lineChart = GenerateLineChart();// создание линейной диаграммы
            //ViewData["LineChart"] = lineChart;

            //Chart horBarChart = GenerateHorizontalBarChart();// создание горизонтальной диаграммы
            //ViewData["horBarChart"] = horBarChart;

            Chart customLineChart = GenerateCustomLineChart();// создание кастомной диаграммы для окупаемости за год
            ViewData["customLineChart"] = customLineChart;

            Chart customVerticalBarChart = GenerateCustomVerticalBarChart(dateCompare);// создание кастомной диаграммы для сравнения за 2 выбранных месяца
            ViewData["customVerticalBarChart"] = customVerticalBarChart;

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
                TempData["SearchMessage"] = $"По вашему запросу найдено {NewFullCars.Count} авто";
                return View("CarView", NewFullCars);
            }
            if(SearchPrice != "")
            {
                NewFullCars = cars.Where(s => s.CarPrice >= price).DistinctBy(s => s.CarName).ToList();
                TempData["SearchMessage"] = $"По вашему запросу найдено {NewFullCars.Count} авто";
                return View("CarView", NewFullCars);
            }
            else
            {
                NewFullCars = cars.DistinctBy(s => s.CarName).ToList();
                TempData["SearchMessage"] = $"По вашему запросу найдено ничего не было найдено";
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

        #region Круговая диаграмма
        private static Chart GeneratePieChart()
        {
            List<OrderApi> ordersData = GetOrder().Result;
            List<CarApi> carsData = GetCars().Result;

            List<double?> datas = ChartsData.GetPieData(ordersData);////////////////////////////////////////
            List<string?> labels = ChartsData.GetPieDataLabels(ordersData, CarsChart);////////////////////////////////////////

            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Pie;

            ChartJSCore.Models.Data data = new Data();
            data.Labels = labels;// Названия для легенды

            PieDataset dataset = new PieDataset()
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>() {  // цвет при выводе
                        ChartColor.FromHexString("#FF6384"),
                        ChartColor.FromHexString("#36A2EB"),
                        ChartColor.FromHexString("#FFCE56"),
                        ChartColor.FromHexString("#9966CC"),
                        ChartColor.FromHexString("#BDECB6"),
                    },
                HoverBackgroundColor = new List<ChartColor>() { // цвет при наведении
                        ChartColor.FromHexString("#FF6384"),
                        ChartColor.FromHexString("#36A2EB"),
                        ChartColor.FromHexString("#FFCE56"),
                        ChartColor.FromHexString("#9966CC"),
                        ChartColor.FromHexString("#BDECB6")
                    },
                Data = datas// заполнение данными
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);                   // добавление данных

            chart.Data = data;


            return chart;
        }
        #endregion

        private static Chart GenerateVerticalBarChart()
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Bar;

            Data data = new Data();
            data.Labels = new List<string>() { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };

            BarDataset dataset = new BarDataset()
            {
                Label = "Первые данные",
                Data = new List<double?>() { 12, 17, 3, 5, 2, 3 },
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromRgba(255, 99, 132, 0.2),
                    ChartColor.FromRgba(54, 162, 235, 0.2),
                    ChartColor.FromRgba(255, 206, 86, 0.2),
                    ChartColor.FromRgba(75, 192, 192, 0.2),
                    ChartColor.FromRgba(153, 102, 255, 0.2),
                    ChartColor.FromRgba(255, 159, 64, 0.2)
                },
                BorderColor = new List<ChartColor>
                {
                    ChartColor.FromRgb(255, 99, 132),
                    ChartColor.FromRgb(54, 162, 235),
                    ChartColor.FromRgb(255, 206, 86),
                    ChartColor.FromRgb(75, 192, 192),
                    ChartColor.FromRgb(153, 102, 255),
                    ChartColor.FromRgb(255, 159, 64)
                },
                BorderWidth = new List<int>() { 1, 2 }, // толщина столбиков
                BarPercentage = 10,  // хз
                BarThickness = 8,     // ширина столбиков
                MaxBarThickness = 12,  // максимальная ширина столбиков
                MinBarLength = 1      // минимальная длина 
            };

            BarDataset dataset2 = new BarDataset()
            {
                Label = "Вторые данные",
                Data = new List<double?>() { 4, 8, 1, 12, 9, 7 },
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromRgba(255, 99, 132, 0.2),
                    ChartColor.FromRgba(54, 162, 235, 0.2),
                    ChartColor.FromRgba(255, 206, 86, 0.2),
                    ChartColor.FromRgba(75, 192, 192, 0.2),
                    ChartColor.FromRgba(153, 102, 255, 0.2),
                    ChartColor.FromRgba(255, 159, 64, 0.2)
                },
                BorderColor = new List<ChartColor>
                {
                    ChartColor.FromRgb(255, 99, 132),
                    ChartColor.FromRgb(54, 162, 235),
                    ChartColor.FromRgb(255, 206, 86),
                    ChartColor.FromRgb(75, 192, 192),
                    ChartColor.FromRgb(153, 102, 255),
                    ChartColor.FromRgb(255, 159, 64)
                },
                BorderWidth = new List<int>() { 1, 2 }, // толщина столбиков
                BarPercentage = 10,  // хз
                BarThickness = 8,     // ширина столбиков
                MaxBarThickness = 12,  // максимальная ширина столбиков
                MinBarLength = 100      // минимальная длина 
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);
            data.Datasets.Add(dataset2);

            chart.Data = data;

            var options = new Options
            {
                Scales = new Dictionary<string, Scale>()
                {
                    { "y", new CartesianLinearScale()
                        {
                            BeginAtZero = true
                        }
                    },
                    { "x", new Scale()
                        {
                            Grid = new Grid()
                            {
                                Offset = true
                            }
                        }
                    },
                }
            };

            chart.Options = options;

            chart.Options.Layout = new Layout
            {
                Padding = new Padding
                {
                    PaddingObject = new PaddingObject
                    {
                        Left = 10,
                        Right = 12
                    }
                }
            };


            return chart;
        }

        private static Chart GenerateLineChart()
        {
            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Line;
            chart.Options.Scales = new Dictionary<string, Scale>();
            CartesianScale xAxis = new CartesianScale();
            xAxis.Display = true;
            xAxis.Title = new Title
            {
                Text = new List<string> { "Month" },
                Display = true
            };
            chart.Options.Scales.Add("x", xAxis);


            Data data = new Data
            {
                Labels = new List<string> { "January", "February", "March", "April", "May", "June", "July" }
            };

            LineDataset dataset = new LineDataset()
            {
                Label = "My First dataset",
                Data = new List<double?> { 65, 59, 80, 81, 56, 55, 40 },
                Fill = "true",
                Tension = .01,
                BackgroundColor = new List<ChartColor> { ChartColor.FromRgba(75, 192, 192, 0.4) },
                BorderColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                BorderCapStyle = "butt",
                BorderDash = new List<int>(),
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                PointBackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#ffffff") },
                PointBorderWidth = new List<int> { 1 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                PointHoverBorderColor = new List<ChartColor> { ChartColor.FromRgb(220, 220, 220) },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };

            data.Datasets = new List<Dataset>
            {
                dataset
            };

            chart.Data = data;

            ZoomOptions zoomOptions = new ZoomOptions
            {
                Zoom = new Zoom
                {
                    Wheel = new Wheel
                    {
                        Enabled = true
                    },
                    Pinch = new Pinch
                    {
                        Enabled = true
                    },
                    Drag = new Drag
                    {
                        Enabled = true,
                        ModifierKey = Enums.ModifierKey.alt
                    }
                },
                Pan = new Pan
                {
                    Enabled = true,
                    Mode = "xy"
                }
            };

            chart.Options.Plugins = new ChartJSCore.Models.Plugins
            {
                PluginDynamic = new Dictionary<string, object> { { "zoom", zoomOptions } }
            };

            return chart;
        }

        private static Chart GenerateHorizontalBarChart()
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Bar;

            chart.Data = new Data()
            {
                Datasets = new List<Dataset>()
                {
                    {
                        new VerticalBarDataset()
                        {
                            Label = "Dataset 1",
                            Data = new List<VerticalBarDataPoint?>()
                            {
                                new VerticalBarDataPoint(12, 1)
                            },
                            BackgroundColor = new List<ChartColor>
                            {
                                ChartColor.FromRgba(255, 99, 132, 0.2)
                            },
                            BorderWidth = new List<int>() { 2 },
                            IndexAxis = "y"
                        }
                    },
                    {
                        new VerticalBarDataset()
                        {
                            Label = "Dataset 2",
                            Data = new List<VerticalBarDataPoint?>()
                            {
                                new VerticalBarDataPoint(19, 2)
                            },
                            BackgroundColor = new List<ChartColor>
                            {
                                ChartColor.FromRgba(54, 162, 235, 0.2)
                            },
                            BorderWidth = new List<int>() { 2 },
                            IndexAxis = "y"
                        }
                    },
                    {
                        new VerticalBarDataset()
                        {
                            Label = "Dataset 3",
                            Data = new List<VerticalBarDataPoint?>()
                            {
                                new VerticalBarDataPoint(3, 3)
                            },
                            BackgroundColor = new List<ChartColor>
                            {
                                ChartColor.FromRgba(255, 206, 86, 0.2)
                            },
                            BorderWidth = new List<int>() { 2 },
                            IndexAxis = "y"
                        }
                    },
                    {
                        new VerticalBarDataset()
                        {
                            Label = "Dataset 4",
                            Data = new List<VerticalBarDataPoint?>()
                            {
                                new VerticalBarDataPoint(5, 4)
                            },
                            BackgroundColor = new List<ChartColor>
                            {
                                ChartColor.FromRgba(75, 192, 192, 0.2)
                            },
                            BorderWidth = new List<int>() { 2 },
                            IndexAxis = "y"
                        }
                    },
                    {
                        new VerticalBarDataset()
                        {
                            Label = "Dataset 5",
                            Data = new List<VerticalBarDataPoint?>()
                            {
                                new VerticalBarDataPoint(2, 5)
                            },
                            BackgroundColor = new List<ChartColor>
                            {
                                ChartColor.FromRgba(153, 102, 255, 0.2)
                            },
                            BorderWidth = new List<int>() { 2 },
                            IndexAxis = "y"
                        }
                    },
                    {
                        new VerticalBarDataset()
                        {
                            Label = "Dataset 6",
                            Data = new List<VerticalBarDataPoint?>()
                            {
                                new VerticalBarDataPoint(-3, 6)
                            },
                            BackgroundColor = new List<ChartColor>
                            {
                                ChartColor.FromRgba(255, 159, 64, 0.2)
                            },
                            BorderWidth = new List<int>() { 2 },
                            IndexAxis = "y"
                        }
                    }
                }
            };

            chart.Options = new Options()
            {
                Responsive = true,
                Plugins = new ChartJSCore.Models.Plugins()
                {
                    Legend = new Legend()
                    {
                        Position = "right"
                    },
                    Title = new Title()
                    {
                        Display = true,
                        Text = new List<string>() { "Chart.js Horizontal Bar Chart" }
                    }
                }
            };

            return chart;
        }
        
        private static async Task<List<OrderApi>> GetOrder()
        {
            Orders1 = await Api.GetListAsync<List<OrderApi>>("Order");
            return Orders1;
        }

        private static async Task<List<CarApi>> GetCars()
        {
            CarsChart = await Api.GetListAsync<List<CarApi>>("Car");
            return CarsChart;
        }

        private static Chart GenerateCustomLineChart()
        {
            List<double?> datas = ChartsData.GetProfitChartData();// получение прибыли по месяцам в течение года

            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Line;
            chart.Options.Scales = new Dictionary<string, Scale>();
            CartesianScale xAxis = new CartesianScale();
            xAxis.Display = true;
            xAxis.Title = new Title
            {
                Text = new List<string> { "Месяц" },
                Display = true,
                Color = ChartColor.FromHexString("#00a550"),
                Font = new Font()
                {
                    Family = "Comic Sans MS",
                    Size = 20,
                    Weight = "Bold",
                    LineHeight = "1.2"
                }
            };

            CartesianScale yAxis = new CartesianScale();
            yAxis.Display = true;
            yAxis.Title = new Title
            {
                Text = new List<string> { "Доходы, руб." },
                Display = true,
                Color = ChartColor.FromHexString("#00a550"),
                Font = new Font()
                {
                    Family = "Comic Sans MS",
                    Size = 20,
                    Weight = "Bold",
                    LineHeight = "1.2"
                }
            };
            chart.Options.Scales.Add("x", xAxis);
            chart.Options.Scales.Add("y", yAxis);


            Data data = new Data
            {
                Labels = new List<string> { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" }
            };





            LineDataset dataset = new LineDataset()
            {
                Label = "Доход по месяцам",
                Data = datas,
                //Fill = "true",
                Tension = .01,
                BackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#ff2b2b") },
                BorderColor = new List<ChartColor> { ChartColor.FromHexString("#ff2b2b") },
                BorderCapStyle = "butt",
                BorderDash = new List<int>(),
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<ChartColor> { ChartColor.FromHexString("#ff2b2b") },
                PointBackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#ffffff") },
                PointBorderWidth = new List<int> { 1 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                PointHoverBorderColor = new List<ChartColor> { ChartColor.FromRgb(220, 220, 220) },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };

            //LineDataset dataset2 = new LineDataset()
            //{
            //    Label = "My First dataset",
            //    Data = new List<double?> { 44, 52, 80, 65, 67, 43, 38 },
            //    //Fill = "true",
            //    Tension = .01,
            //    BackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#000080") },
            //    BorderColor = new List<ChartColor> { ChartColor.FromHexString("#000080") },
            //    BorderCapStyle = "butt",
            //    BorderDash = new List<int>(),
            //    BorderDashOffset = 0.0,
            //    BorderJoinStyle = "miter",
            //    PointBorderColor = new List<ChartColor> { ChartColor.FromHexString("#000080") },
            //    PointBackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#ffffff") },
            //    PointBorderWidth = new List<int> { 1 },
            //    PointHoverRadius = new List<int> { 5 },
            //    PointHoverBackgroundColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
            //    PointHoverBorderColor = new List<ChartColor> { ChartColor.FromRgb(220, 220, 220) },
            //    PointHoverBorderWidth = new List<int> { 2 },
            //    PointRadius = new List<int> { 1 },
            //    PointHitRadius = new List<int> { 10 },
            //    SpanGaps = false
            //};

            data.Datasets = new List<Dataset>
            {
                dataset
                //dataset2
            };

            chart.Data = data;

            ZoomOptions zoomOptions = new ZoomOptions
            {
                Zoom = new Zoom
                {
                    Wheel = new Wheel
                    {
                        Enabled = true
                    },
                    Pinch = new Pinch
                    {
                        Enabled = true
                    },
                    Drag = new Drag
                    {
                        Enabled = true,
                        ModifierKey = Enums.ModifierKey.alt
                    }
                },
                Pan = new Pan
                {
                    Enabled = true,
                    Mode = "xy"
                }
            };

            chart.Options.Plugins = new ChartJSCore.Models.Plugins
            {
                PluginDynamic = new Dictionary<string, object> { { "zoom", zoomOptions } }
            };

            return chart;
        }

        public async Task<IActionResult> CompareDatas(DateTime dateCompare)
        {


            List<double?> datas = ChartsData.GetProfitCompareMonth(dateCompare);


            return View("DashBoardView");
        }

        private static Chart GenerateCustomVerticalBarChart(DateTime date)
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Bar;

            List<double?> datas = ChartsData.GetProfitCompareMonth(date);//

            chart.Data = new Data()
            {
                Datasets = new List<Dataset>()
                {
                    {
                        new VerticalBarDataset()
                        {
                            Label = "Текущий месяц",
                            Data = new List<VerticalBarDataPoint?>()
                            {
                                new VerticalBarDataPoint(datas.First().Value, 1)
                            },
                            BackgroundColor = new List<ChartColor>
                            {
                                ChartColor.FromRgba(255, 99, 132, 0.2)
                            },
                            BorderWidth = new List<int>() { 2 },
                            IndexAxis = "y"
                        }
                    },
                    {
                        new VerticalBarDataset()
                        {
                            Label = "Предыдущий месяц",
                            Data = new List<VerticalBarDataPoint?>()
                            {
                                new VerticalBarDataPoint(datas.Last().Value, 2)
                            },
                            BackgroundColor = new List<ChartColor>
                            {
                                ChartColor.FromRgba(54, 162, 235, 0.2)
                            },
                            BorderWidth = new List<int>() { 2 },
                            IndexAxis = "y"
                        }
                    },

                }
            };

            chart.Options = new Options()
            {
                Responsive = true,
                Plugins = new ChartJSCore.Models.Plugins()
                {
                    Legend = new Legend()
                    {
                        Position = "right"
                    },
                    Title = new Title()
                    {
                        Display = true,
                        Text = new List<string>() { "Сравнение прибыли" }
                    }
                }
            };

            return chart;
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