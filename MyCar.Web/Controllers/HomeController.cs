using ChartJSCore.Helpers;
using ChartJSCore.Models;
using ChartJSCore.Plugins.Zoom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using ModelsApi;
using MyCar.Server.DB;
using MyCar.Web.Core;
using MyCar.Web.Core.Paging;
using MyCar.Web.Models;
using SmartBreadcrumbs.Attributes;
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

        public List<SaleCarApi> Cars { get; set; }
        public List<MarkCarApi> Marks { get; set; }
        public static List<OrderApi> Orders { get; set; }

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

        public async Task<IActionResult> ElectricCarView()
        {
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            return View("ElectricCarView", cars);
        }

        public async Task<IActionResult> LexusGXView()
        {
            return View("LexusGXView");
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
            Chart pieChart = GeneratePieChart();// создание круговой диаграммы
            ViewData["PieChart"] = pieChart;

            Chart vertBarChart = GenerateVerticalBarChart();// создание вертикальной диаграммы
            ViewData["VertBarChart"] = vertBarChart;


            Chart lineChart = GenerateLineChart();// создание линейной диаграммы
            ViewData["LineChart"] = lineChart;
            
            Chart horBarChart = GenerateHorizontalBarChart();// создание горизонтальной диаграммы
            ViewData["horBarChart"] = horBarChart;

            Chart customLineChart = GenerateCustomLineChart();// создание кастомной диаграммы
            ViewData["customLineChart"] = customLineChart;

            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Breadcrumb(FromAction = "Index", Title = "CarView")]
        [HttpGet]
        public async Task<IActionResult> CarView()//int? pageNumber
        {
            List<SaleCarApi> cars;
            List<MarkCarApi> markCars;
            cars = GetCar().Result;
            markCars = GetMark().Result;
            ViewBag.MarkCars = markCars;
            foreach(var car in cars)
            {
                GetDiscount(car);
            }
            return View("CarView", cars);
        }

        [Breadcrumb(FromAction = "Index", Title = "CarView")]
        [HttpGet]
        public async Task<IActionResult> SearchCar(string SearchString)
        {
            List<SaleCarApi> cars;
            List<MarkCarApi> markCars;
            cars = GetCar().Result;
            markCars = GetMark().Result;
            ViewBag.MarkCars = markCars;
            string type = "Авто";
            string filter = "Все";
            cars = cars.Where(s => s.Car.CarName.ToLower().Contains(SearchString.ToLower())).ToList();
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

        private static async Task<List<OrderApi>> GetOrder()
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            return Orders;
        }

        private void GetDiscount(SaleCarApi saleCars)
        {
            var date = DateTime.Now;
            if(saleCars != null)
            {
                if(date.DayOfWeek == DayOfWeek.Monday)
                {
                    //ViewBag.DiscountPrice = 
                    if (saleCars.Car.CarMark.Contains("Toyota") || saleCars.Car.CarMark.Contains("Lexus") || saleCars.Car.CarMark.Contains("Honda"))
                    {
                        ViewBag.DiscountPrice = saleCars.FullPrice * 10 / 100;
                    }
                    else
                    {
                        ViewBag.DiscountPrice = "";
                    }
                }
                else if(date.DayOfWeek == DayOfWeek.Tuesday || date.DayOfWeek == DayOfWeek.Friday)
                {
                    if (saleCars.Car.CarMark.Contains("Porsche") || saleCars.Car.CarMark.Contains("Audi") || saleCars.Car.CarMark.Contains("Honda"))
                    {
                        var diff = saleCars.FullPrice * 10 / 100;
                        ViewBag.DiscountPrice = saleCars.FullPrice - diff;
                    }
                    else
                    {
                        ViewBag.DiscountPrice = "";
                    }
                }
                else
                {
                    ViewBag.DiscountPrice = "";
                }

            }
            
        }

        #region Круговая диаграмма
        private static Chart GeneratePieChart()
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Pie;

            ChartJSCore.Models.Data data = new Data();
            data.Labels = new List<string>() { "Red", "Blue", "Yellow" };// Названия для легенды

            PieDataset dataset = new PieDataset()
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>() {  // цвет при выводе
                        ChartColor.FromHexString("#FF6384"),
                        ChartColor.FromHexString("#36A2EB"),
                        ChartColor.FromHexString("#FFCE56")
                    },
                HoverBackgroundColor = new List<ChartColor>() { // цвет при наведении
                        ChartColor.FromHexString("#FF6384"),
                        ChartColor.FromHexString("#36A2EB"),
                        ChartColor.FromHexString("#FFCE56")
                    },
                Data = new List<double?>() { 300, 50, 100 }// заполнение данными
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


        private static Chart GenerateCustomLineChart()
        {
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
                Text = new List<string> { "Заказы" },
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
                Labels = new List<string> { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль" }
            };

            LineDataset dataset = new LineDataset()
            {
                Label = "My First dataset",
                Data = new List<double?> { 65, 59, 80, 81, 56, 55, 40 },
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

            LineDataset dataset2 = new LineDataset()
            {
                Label = "My First dataset",
                Data = new List<double?> { 44, 52, 80, 65, 67, 43, 38 },
                //Fill = "true",
                Tension = .01,
                BackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#000080") },
                BorderColor = new List<ChartColor> { ChartColor.FromHexString("#000080") },
                BorderCapStyle = "butt",
                BorderDash = new List<int>(),
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<ChartColor> { ChartColor.FromHexString("#000080") },
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
                dataset,
                dataset2
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

            return chart;
        }
    }
}