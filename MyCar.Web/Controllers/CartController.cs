using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelsApi;
using MyCar.Server.DB;
using MyCar.Web.Core;
using MyCar.Web.Models.Payments.Contracts;
using MyCar.Web.Models.Payments.Stripe;
using Newtonsoft.Json;
using Spire.Xls;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace MyCar.Web.Controllers
{
    public class CartController : Controller
    {
        public List<OrderApi> Orders = new List<OrderApi>();
        public List<OrderApi> SearchOrders = new List<OrderApi>();
        public List<SaleCarApi> Cars { get; set; } = new List<SaleCarApi>();

        public DateTime DateStart { get; set; }
        public DateTime DateFinish { get; set; }

        public int TotalCount = 0;

        public List<StatusApi> Statuses { get; set; } = new List<StatusApi>();

        public List<UserApi> Users { get; set; } = new List<UserApi>();

        public List<ActionTypeApi> Types { get; set; }
        public List<WareHouseApi> Warehouses { get; set; } = new List<WareHouseApi>();
        public List<CountChangeHistoryApi> CountChangeHistories { get; set; } = new List<CountChangeHistoryApi>();

        // GET: CartController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CartController/Details/5

        // GET: CartController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CartController/Create
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

        [Authorize(Roles = "Администратор, Клиент")]
        public IActionResult CartPage(string name)
        {
            name = User.Identity.Name;
            
            return View("CartPage", GetPage(name).Result);
        }

        public async Task<IActionResult> SearchOrder(string FilterString)
        {
            string name = User.Identity.Name;
            if (User.IsInRole("Администратор"))
            {
                Orders = await Api.GetListAsync<List<OrderApi>>("Order");
                SearchOrders = Orders.Where(s => s.Status.StatusName.Contains(FilterString)).ToList();
                return View("CartPage", SearchOrders);
            }
            if (User.IsInRole("Клиент"))
            {
                Orders = await Api.GetListAsync<List<OrderApi>>("Order");
                SearchOrders = Orders.Where(s => s.Status.StatusName.Contains(FilterString) && s.User.UserName.Contains(name)).ToList();
                return View("CartPage", SearchOrders);
            }
            return View("CartPage", SearchOrders);
        }

        public IActionResult Report(string name)
        {
            //Task.Run(GetOrder);
            Orders = GetOrder().Result;
            GenerateReport(Orders);
            return View("CartPage", GetPage(name).Result);
        }

        public void GenerateReport(List<OrderApi> orders)
        {
            decimal TotalPurchasePrice = 0;
            decimal TotalRawSalePrice = 0;
            decimal TotalSalePrice = 0;
            decimal TotalProfit = 0;
            Warehouses = GetWarehouses().Result;
            CountChangeHistories = GetHistory().Result;
            var filterOrders = orders.Where(s=> s.WareHouses.Count != 0 && s.ActionType.ActionTypeName == "Продажа" && s.Status.StatusName == "Завершен").ToList();
            if (User.IsInRole("Администратор"))
            {
                var workbook = new Workbook();
                var sheet = workbook.Worksheets[0];
                sheet.Range["A1"].Value = "ПРОДАЖИ";
                sheet.Range["A1:B1"].Merge();
                sheet.Range["D1"].Value = "С";
                sheet.Range["E1"].Value = DateStart.ToShortDateString();
                sheet.Range["F1"].Value = "ПО";
                sheet.Range["G1"].Value = DateFinish.ToShortDateString();
                sheet.Range["D1:M1"].Style.HorizontalAlignment = HorizontalAlignType.Center;

                sheet.Range["A3"].Value = "Артикул";
                sheet.Range["B3"].Value = "Дата";
                sheet.Range["C3"].Value = "Наименование";
                sheet.Range["D3"].Value = "Кол-во";
                sheet.Range["E3"].Value = "Закупочная цена";
                sheet.Range["F3"].Value = "Цвет";
                sheet.Range["G3"].Value = "Комплектация";
                sheet.Range["H3"].Value = "Цена комплектации";
                sheet.Range["I3"].Value = "Цена продажи";
                sheet.Range["J3"].Value = "Cкидка";
                sheet.Range["K3"].Value = "Сумма со скидкой";
                sheet.Range["L3"].Value = "Прибыль";
                sheet.Range["M3"].Value = "Покупатель";

                var index = 4;

                foreach (var order in filterOrders)
                {
                    foreach(var warehouse in order.WareHouses)
                    {
                        decimal purchase = 0;

                        sheet.Range[$"A{index}"].Value = warehouse.SaleCar.Articul.ToString();
                        sheet.Range[$"B{index}"].Value = order.DateOfOrder.ToString().Substring(0, order.DateOfOrder.ToString().Length - 8);
                        sheet.Range[$"C{index}"].Value = warehouse.SaleCar.Car.CarName.ToString();

                        sheet.Range[$"D{index}"].Value = (warehouse.CountChange * -1).ToString();
                        TotalCount += (int)warehouse.CountChange;

                        purchase = (decimal)orders.SelectMany(s => s.WareHouses).FirstOrDefault(s => s.ID == CountChangeHistories?.FirstOrDefault(s => s?.WarehouseOutId == warehouse.ID).WarehouseInId).Price;
                        sheet.Range[$"E{index}"].Value = purchase.ToString();
                        sheet.Range[$"E{index}"].NumberFormat = "0.00 ₽";
                        TotalPurchasePrice += (decimal)(purchase * warehouse.CountChange * -1);

                        sheet.Range[$"F{index}"].Value = warehouse.SaleCar.ColorCar.ToString();
                        sheet.Range[$"G{index}"].Value = warehouse.SaleCar.Equipment.NameEquipment.ToString();
                        sheet.Range[$"H{index}"].Value = warehouse.SaleCar.EquipmentPrice.ToString();
                        sheet.Range[$"H{index}"].NumberFormat = "0.00 ₽";

                        sheet.Range[$"I{index}"].Value = warehouse.Price.ToString();
                        sheet.Range[$"I{index}"].NumberFormat = "0.00 ₽";
                        TotalRawSalePrice += (decimal)(warehouse.Price * warehouse.CountChange * -1);

                        sheet.Range[$"J{index}"].Value = warehouse.Discount.ToString();
                        sheet.Range[$"J{index}"].NumberFormat = "0.00 ₽";


                        sheet.Range[$"K{index}"].Value = ((warehouse.Price - warehouse.Discount) * warehouse.CountChange * -1).ToString();
                        sheet.Range[$"K{index}"].NumberFormat = "0.00 ₽";
                        TotalSalePrice += (decimal)((warehouse.Price - warehouse.Discount) * warehouse.CountChange * -1);


                        sheet.Range[$"L{index}"].Value = ((warehouse.Price - warehouse.Discount - purchase) * warehouse.CountChange).ToString();
                        sheet.Range[$"L{index}"].NumberFormat = "0.00 ₽";
                        TotalProfit += (decimal)((warehouse.Price - warehouse.Discount - purchase) * warehouse.CountChange * -1);

                        sheet.Range[$"M{index}"].Value = order.User.UserName;

                        index++;
                    }
                }

                sheet.Range[$"A{index}"].Value = "Всего";
                sheet.Range[$"A{index}:C{index}"].Merge();
                sheet.Range[$"D{index}"].Value = TotalCount.ToString();

                sheet.Range[$"E{index}"].Value = TotalPurchasePrice.ToString();
                sheet.Range[$"E{index}"].NumberFormat = "0.00 ₽";

                sheet.Range[$"F{index}:H{index}"].Style.Color = System.Drawing.Color.Gray;

                sheet.Range[$"I{index}"].Value = TotalRawSalePrice.ToString();
                sheet.Range[$"I{index}"].NumberFormat = "0.00 ₽";

                sheet.Range[$"J{index}"].Style.Color = System.Drawing.Color.Gray;

                sheet.Range[$"K{index}"].Value = TotalSalePrice.ToString();
                sheet.Range[$"K{index}"].NumberFormat = "0.00 ₽";

                sheet.Range[$"L{index}"].Value = TotalProfit.ToString();
                sheet.Range[$"L{index}"].NumberFormat = "0.00 ₽";

                sheet.Range[$"A3:M{index}"].BorderInside(LineStyleType.Thin);
                sheet.Range[$"A3:M{index}"].BorderAround(LineStyleType.Medium);

                sheet.AllocatedRange.AutoFitColumns();

                try
                {
                    workbook.SaveToFile("text.xls");
                    Process p = new Process();
                    p.StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + "/text.xls")
                    {
                        UseShellExecute = true
                    };
                    p.Start();
                }
                catch (Exception ex)
                {
                    View("CartError");
                }
            }
            if (User.IsInRole("Клиент"))
            {
                var workbook = new Workbook();
                var sheet = workbook.Worksheets[0];
                var userOrders = filterOrders.Where(s=> s.User.UserName == User.Identity.Name);
                sheet.Range["A1"].Value = $"Заказы пользователя: {User.Identity.Name}";
                sheet.Range["A1:B1"].Merge();
                sheet.Range["D1"].Value = "С";
                sheet.Range["E1"].Value = DateStart.ToShortDateString();
                sheet.Range["F1"].Value = "ПО";
                sheet.Range["G1"].Value = DateFinish.ToShortDateString();
                sheet.Range["D1:M1"].Style.HorizontalAlignment = HorizontalAlignType.Center;

                sheet.Range["A3"].Value = "Артикул";
                sheet.Range["B3"].Value = "Дата";
                sheet.Range["C3"].Value = "Наименование";
                sheet.Range["D3"].Value = "Кол-во";
                sheet.Range["E3"].Value = "Цвет";
                sheet.Range["F3"].Value = "Комплектация";
                sheet.Range["G3"].Value = "Цена комплектации";
                sheet.Range["H3"].Value = "Цена продажи";
                sheet.Range["I3"].Value = "Cкидка";
                sheet.Range["J3"].Value = "Сумма со скидкой";
                sheet.Range["K3"].Value = "Покупатель";

                var index = 4;

                foreach (var order in userOrders)
                {
                    foreach (var warehouse in order.WareHouses)
                    {
                        decimal purchase = 0;

                        sheet.Range[$"A{index}"].Value = warehouse.SaleCar.Articul.ToString();
                        sheet.Range[$"B{index}"].Value = order.DateOfOrder.ToString().Substring(0, order.DateOfOrder.ToString().Length - 8);
                        sheet.Range[$"C{index}"].Value = warehouse.SaleCar.Car.CarName.ToString();

                        sheet.Range[$"D{index}"].Value = warehouse.CountChange.ToString();
                        TotalCount += (int)warehouse.CountChange;

                        sheet.Range[$"E{index}"].Value = warehouse.SaleCar.ColorCar.ToString();
                        sheet.Range[$"F{index}"].Value = warehouse.SaleCar.Equipment.NameEquipment.ToString();
                        sheet.Range[$"G{index}"].Value = warehouse.SaleCar.EquipmentPrice.ToString();
                        sheet.Range[$"G{index}"].NumberFormat = "0.00 ₽";

                        sheet.Range[$"H{index}"].Value = warehouse.Price.ToString();
                        sheet.Range[$"H{index}"].NumberFormat = "0.00 ₽";
                        TotalRawSalePrice += (decimal)(warehouse.Price * warehouse.CountChange * -1);

                        sheet.Range[$"I{index}"].Value = warehouse.Discount.ToString();
                        sheet.Range[$"I{index}"].NumberFormat = "0.00 ₽";


                        sheet.Range[$"J{index}"].Value = ((warehouse.Price - warehouse.Discount) * warehouse.CountChange * -1).ToString();
                        sheet.Range[$"J{index}"].NumberFormat = "0.00 ₽";
                        TotalSalePrice += (decimal)((warehouse.Price - warehouse.Discount) * warehouse.CountChange * -1);


                        sheet.Range[$"K{index}"].Value = ((warehouse.Price - warehouse.Discount - purchase) * warehouse.CountChange).ToString();
                        sheet.Range[$"K{index}"].NumberFormat = "0.00 ₽";
                        //TotalProfit += (decimal)((warehouse.Price - warehouse.Discount - purchase) * warehouse.CountChange);

                        sheet.Range[$"L{index}"].Value = order.User.UserName;

                        index++;
                    }
                }

                sheet.Range[$"A{index}"].Value = "Всего";
                sheet.Range[$"A{index}:C{index}"].Merge();
                sheet.Range[$"D{index}"].Value = TotalCount.ToString();

                sheet.Range[$"E{index}"].Value = "Сумма: " + TotalRawSalePrice.ToString();
                sheet.Range[$"E{index}"].NumberFormat = "0.00 ₽";

                sheet.Range[$"F{index}:H{index}"].Style.Color = System.Drawing.Color.Gray;

                sheet.Range[$"I{index}"].Value = TotalRawSalePrice.ToString();
                sheet.Range[$"I{index}"].NumberFormat = "0.00 ₽";

                sheet.Range[$"J{index}"].Value = TotalSalePrice.ToString();
                sheet.Range[$"J{index}"].NumberFormat = "0.00 ₽";

                sheet.Range[$"A3:L{index}"].BorderInside(LineStyleType.Thin);
                sheet.Range[$"A3:L{index}"].BorderAround(LineStyleType.Medium);

                sheet.AllocatedRange.AutoFitColumns();

                try
                {
                    workbook.SaveToFile("textUser.xls");
                    Process p = new Process();
                    p.StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + "/textUser.xls")
                    {
                        UseShellExecute = true
                    };
                    p.Start();
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

        public async Task<List<OrderApi>> GetPage(string name)
        {
            await Task.Run(GetUser);
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            var order = Orders.Where(s=> s.ActionType.ActionTypeName == "Продажа" && s.Status.StatusName != "Отменен").ToList();
            foreach (var ord in order)
            {
                ord.SumOrder = ord.WareHouses.Sum(s => s.Price * s.CountChange * -1);
            }
            var userIsAdmin = Users.FirstOrDefault(s => s.UserName == name);
            if (userIsAdmin.UserType.TypeName == "Администратор")
            {
                var users = await Api.GetListAsync<List<UserApi>>("User");
                ViewBag.Users = order.Where(s => s.UserId != 0).Select(s => s.User);
                return order;
            }
            else if (userIsAdmin.UserType.TypeName == "Клиент")
            {
                order = Orders.Where(s => s.User.UserName == userIsAdmin.UserName).ToList();
                return order;
            }
            
            return order;
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

        public async Task<IActionResult> AddCars(int id)
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            var order = Orders.FirstOrDefault(s => s.ID == id);
            return View("DetailsCart", order);
        }

        public async Task<IActionResult> EditWare(int id)
        {
            var orderItems = new List<WareHouseApi>();
            string json = HttpContext.Session.GetString("OrderItem");
            if (json != null)
                orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(json) ?? new List<WareHouseApi>();
            var ware = orderItems.FirstOrDefault(s => s.SaleCarId == id);
            return View("EditCountWareHouse", ware);
        }

        public async Task<IActionResult> EditCountWareHouse(int id, int CountChangeWare)
        {
            await GetOrders();
            var orderItems = new List<WareHouseApi>();
            string json = HttpContext.Session.GetString("OrderItem");
            if (json != null)
                orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(json) ?? new List<WareHouseApi>();
            var car = Cars.FirstOrDefault(s => s.ID == id);
            var wh = orderItems.LastOrDefault(s => s.SaleCarId == id);
            car.CountChange = CountChangeWare;
            wh.CountChange = CountChangeWare;
            if (IsCanAddInOrder(car) == false)
            {
                TempData["OrderCountErrorMessage"] = "Превышено максмиальное кол-во покупок данного авто";
                ViewBag.RecommendCars = Cars.Where(s => s.Car.CarMark.Contains(car.Car.CarMark) && s.ID != car.ID);
                return View("~/Views/Car/DetailsCarView.cshtml", car);
            }
            await EditWareHouse(wh);
            string json1 = JsonConvert.SerializeObject(orderItems);
            var des = JsonConvert.DeserializeObject(json1);

            HttpContext.Session.SetString("OrderItem", json1);
            return View("DetailsCart", orderItems);
        }

        public async Task<IActionResult> DetailsCart(string name)
        {
            List<WareHouseApi> orderItemsOld = new List<WareHouseApi>();
            List<WareHouseApi> orderItems = new List<WareHouseApi>();
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            Warehouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
            Users = await Api.GetListAsync<List<UserApi>>("User");
            var user = Users.FirstOrDefault(s => s.UserName == name);
            // Получаем текущий список из Session
            string json = HttpContext.Session.GetString("OrderItem");
            if (json != null)
                orderItemsOld = JsonConvert.DeserializeObject<List<WareHouseApi>>(json) ?? new List<WareHouseApi>();

            // Получаем текущий список из Session
            string json2 = HttpContext.Session.GetString("OrderItem");
            if (json2 != null)
                orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(json2) ?? new List<WareHouseApi>();
            if(orderItems.Count == 0)
            {
                return View("CartError");
            }

            return View("DetailsCart", orderItems);
        }

        public async Task<IActionResult> DeleteCar(int id)
        {
            List<WareHouseApi> orderItems = new List<WareHouseApi>();
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            string json2 = HttpContext.Session.GetString("OrderItem");
            if (json2 != null)
                orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(json2) ?? new List<WareHouseApi>();
            var deleteCar = orderItems.FirstOrDefault(s => s.SaleCarId == id);
            orderItems.Remove(deleteCar);
            TempData["SaleDeleteMessageMessage"] = $"Авто {deleteCar.SaleCar.FullName} удалено из вашей корзины";
            string json1 = JsonConvert.SerializeObject(orderItems);
            var des = JsonConvert.DeserializeObject(json1);
            HttpContext.Session.SetString("OrderItem", json1);
            return View("DetailsCart", orderItems);
        }

        public async Task<IActionResult> DetailsOrder(int id)
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            Warehouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
            Users = await Api.GetListAsync<List<UserApi>>("User");
            var order = Orders.LastOrDefault(s => s.ID == id);
            if (order == null)
            {
                return View("Error");
            }
            order.WareHouses = Warehouses.Where(s => s.OrderId == order.ID).ToList();
            order.SumOrder = order.WareHouses.Sum(s => s.Price);
            return View("DetailsOrder", order);
        }

        public async Task<IActionResult> AddOrder(int id, int? id2)
        {
            await GetOrders();
            List<WareHouseApi> orderItemsOld = new List<WareHouseApi>();
            List<WareHouseApi> orderItems = new List<WareHouseApi>();
            var car = Cars.FirstOrDefault(s => s.ID == id);
            var price = DiscountCounter.GetDiscount(car);
            if(price == 0)
            {
                price = car.FullPrice;
            }
            var user = Users.FirstOrDefault(s => s.UserName == User.Identity.Name);
            //var warehouses = Warehouses.Where(s => s.OrderId == null || s.OrderId == 0).ToList();
            List<OrderApi> thisOrders = Orders.OrderBy(s => s.DateOfOrder).ToList();
            List<WareHouseApi> WareHouseIns = thisOrders.Where(s => s.ActionType.ActionTypeName == "Поступление").SelectMany(w => w.WareHouses).ToList();
            
            WareHouseApi warehouse = new WareHouseApi
            {
                SaleCarId = id,
                CountChange = id2,
                Discount = 0,
                SaleCar = car,
                Price = price
            };
            // Получаем текущий список из Session
            string json = HttpContext.Session.GetString("OrderItem");
            if (json != null)
                orderItemsOld = JsonConvert.DeserializeObject<List<WareHouseApi>>(json) ?? new List<WareHouseApi>();

            //проверка на схожие авто
            var carSearch = orderItemsOld.FirstOrDefault(s => s.SaleCarId == id);
            if(carSearch != null)
            {
                TempData["SaleExistErrorMessage"] = $"Машина: {carSearch.SaleCar.FullName} уже есть в вашей корзине";
                return View("DetailsCarView", car.ID);
            }

            //проверка на количество
            
            // Добавляем новый объект в список
            orderItemsOld.Add(warehouse);

            // Сохраняем список обратно в Session
            string json1 = JsonConvert.SerializeObject(orderItemsOld);
            var des = JsonConvert.DeserializeObject(json1);
            HttpContext.Session.SetString("OrderItem", json1);

            // Получаем текущий список из Session
            string json2 = HttpContext.Session.GetString("OrderItem");
            if (json2 != null)
                orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(json2) ?? new List<WareHouseApi>();
            return View("DetailsCart", orderItems);
        }

        public async Task<IActionResult> UpdateCountCar(int id, int CountChange)
        {
            await GetOrders();
            var orderItems = new List<WareHouseApi>();
            var car = Cars.FirstOrDefault(s=> s.ID == id);
            car.CountChange = CountChange;
            if(CountChange == 0)
            {
                TempData["OrderCountErrorMessage"] = "Количество покупаемого авто не должно быть равно 0";
                ViewBag.RecommendCars = Cars.Where(s => s.Car.CarMark.Contains(car.Car.CarMark) && s.ID != car.ID);
                return View("~/Views/Car/DetailsCarView.cshtml", car);
            }
            if (IsCanAddInOrder(car) == false)
            {
                TempData["OrderCountErrorMessage"] = "Превышено максмиальное кол-во покупок данного авто";
                ViewBag.RecommendCars = Cars.Where(s => s.Car.CarMark.Contains(car.Car.CarMark) && s.ID != car.ID);
                return View("~/Views/Car/DetailsCarView.cshtml", car);
            }
            await AddOrder(car.ID, car.CountChange);
            string json = HttpContext.Session.GetString("OrderItem");
            if (json != null)
                orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(json) ?? new List<WareHouseApi>();
            return View("DetailsCart", orderItems);
        }

        public async Task<IActionResult> UpdateCountWarehouse(int id, int CountChange)
        {
            await GetOrders();
            var orderItems = new List<WareHouseApi>();
            string json = HttpContext.Session.GetString("OrderItem");
            if (json != null)
                orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(json) ?? new List<WareHouseApi>();
            var car = Cars.FirstOrDefault(s => s.ID == id);
            var wh = orderItems.LastOrDefault(s=> s.SaleCarId == id);
            car.CountChange = CountChange;
            wh.CountChange = CountChange;
            if (IsCanAddInOrder(car) == false)
            {
                TempData["OrderCountErrorMessage"] = "Превышено максмиальное кол-во покупок данного авто";
                ViewBag.RecommendCars = Cars.Where(s => s.Car.CarMark.Contains(car.Car.CarMark) && s.ID != car.ID);
                return View("~/Views/Car/DetailsCarView.cshtml", car);
            }
            await EditWareHouse(wh);
            string json1 = JsonConvert.SerializeObject(orderItems);
            var des = JsonConvert.DeserializeObject(json1);

            HttpContext.Session.SetString("OrderItem", json1);
            return View("DetailsCart", orderItems);
        }

        private readonly IStripeAppService _stripeService;

        public CartController(IStripeAppService stripeService)
        {
            _stripeService = stripeService;
        }
        public async Task<IActionResult> ConfirmOrder()
        {
            await GetOrders();
            var user = Users.FirstOrDefault(s => s.UserName == User.Identity.Name);
            var actionType = Types.FirstOrDefault(s => s.ActionTypeName == "Продажа");
            var status = Statuses.FirstOrDefault(s => s.StatusName == "Ожидает оплаты");
            List<WareHouseApi> orderItems = new List<WareHouseApi>();

            Request.Cookies.TryGetValue("OrderItem", out string answer);

            string json = HttpContext.Session.GetString("OrderItem");
            if (json != null)
                orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(json) ?? new List<WareHouseApi>();
            foreach(var ware in orderItems)
            {
                ware.CountChange = ware.CountChange;
            }
            //OrderItemsFill(orderItems);

            CountCheck(orderItems);

            OrderItemsFill(orderItems);

            OrderApi order = new OrderApi
            {
                UserId = user.ID,
                DateOfOrder = DateTime.Now, 
                ActionTypeId = actionType.ID,
                StatusId = status.ID,
                WareHouses = orderItems,
                ActionType = actionType,
                Status = status,
                User = user
            };

            var marks = new List<MarkCarApi>();
            marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            ViewBag.Marks = marks;
            var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");

            if (order.WareHouses == null || order.WareHouses.Count == 0)
            {
                orderItems.Clear();
                string json4 = JsonConvert.SerializeObject(orderItems);
                HttpContext.Session.SetString("OrderItem", json4);
                TempData["OrderFineMessage"] = $"Что-то пошло не так";
                return View("~/Views/Home/Index.cshtml", cars);
            }

            await CreateOrder(order);
            
            orderItems.Clear();
            string json1 = JsonConvert.SerializeObject(orderItems);
            HttpContext.Session.SetString("OrderItem", json1);

            EmailSender emailSender = new EmailSender();
            emailSender.SendEmailAsync(order.User.UserName, order.User.Email, "Пользователь купил авто", "Пользователь купил авто");
            
            TempData["OrderFineMessage"] = $"Ваш заказ ожидает оплаты";
            return View("~/Views/Home/Index.cshtml", cars);
        }

        public async Task<IActionResult> Payment(int Id)
        {
            await GetOrders();
            var user = Users.FirstOrDefault(s => s.UserName == User.Identity.Name);
            var actionType = Types.FirstOrDefault(s => s.ActionTypeName == "Продажа");
            var status = Statuses.FirstOrDefault(s => s.StatusName == "Завершен");
            var order = Orders.FirstOrDefault(s => s.ID == Id);
            order.User = user;
            order.ActionType = actionType;
            order.Status = status;
            order.StatusId = status.ID;
            if (order == null)
            {
                TempData["OrderMessage"] = "Такого заказа не существует!";
                return View("CartPage", Orders);
            }
            return View("PaymentCart", order);
        }

        public async Task<IActionResult> PaymentOrder(int Id, string cardName, string cardNumber, string cardMonth, string cardYear, string cardCVV)
        {
            if(Id != 0)
            {
                await GetOrders();
                var user = Users.FirstOrDefault(s => s.UserName == User.Identity.Name);
                var actionType = Types.FirstOrDefault(s => s.ActionTypeName == "Продажа");
                var status = Statuses.FirstOrDefault(s => s.StatusName == "Завершен");

                var order = Orders.FirstOrDefault(s => s.ID == Id);
            
                decimal? sum = new();
                StringBuilder sb = new StringBuilder();
                foreach (var item in order?.WareHouses)
                {
                    sum += item.Price;
                    sb.AppendLine("Машина: " + item.SaleCar.FullName + " Количество: " + item.CountChange * -1);
                }
                long totalSum = (long)sum * 100;

                Models.Payments.Stripe.AddStripeCard card = new Models.Payments.Stripe.AddStripeCard(cardName, cardNumber, cardYear, cardMonth, cardCVV);
                Models.Payments.Stripe.AddStripeCustomer customer = new Models.Payments.Stripe.AddStripeCustomer(user.Email, user.UserName, card);
                CancellationToken ct = new CancellationToken();
                StripeCustomer createdCustomer = await _stripeService.AddStripeCustomerAsync(customer, ct);

                long dollarSum = totalSum / 80;
                try
                {
                    Models.Payments.Stripe.AddStripePayment payment = new AddStripePayment(createdCustomer.CustomerId, user.Email, sb.ToString(), "USD", dollarSum);
                    StripePayment createdPayment = await _stripeService.AddStripePaymentAsync(payment, ct);
                    order.Status = status;
                    order.StatusId = status.ID;
                }
                catch (Exception)
                {
                    TempData["ErrorPaymentMessage"] = "При оплате произошел сбой!";
                    return View("PaymentOrder", order);
                }
            
                await EditOrder(order);

                //orderItems.Clear();

                var marks = new List<MarkCarApi>();
                marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
                ViewBag.Marks = marks;
                var cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
                TempData["SuccesPaymentMessage"] = "Оплата проведена успешно!";
                return View("~/Views/Home/Index.cshtml", cars);
            }
            else
            {
                TempData["ErrorPaymentMessage"] = "При оплате произошел сбой!";
                var order = Orders.FirstOrDefault(s => s.ID == Id);
                return View("PaymentOrder", order);
            }
        }



        //public async Task CustomerAdd(AddStripeCustomer customer)
        //{
        //    var order = await Api.PostAsync<AddStripeCustomer>(customer, "Stripe/customer/add");
        //}

        private void CountCheck(List<WareHouseApi> wareHouses)
        {
            foreach (var ware in wareHouses)
            {
                if (ware.CountChange > ware.SaleCar.Count)
                {
                    wareHouses.Remove(ware);
                    //чето делаем
                }
            }
        }
        private void OrderItemsFill(List<WareHouseApi> orderItems)
        {
            //выбираем не отмененные заказы
            List<OrderApi> orders = Orders.Where(o => o.Status.StatusName != "Отменен").ToList();

            //сортируем что бы сначала стояли старые заказы (по методу FIFO(первым пришел - первым ушел) https://ru.wikipedia.org/wiki/FIFO)
            List<OrderApi> thisOrders = orders.OrderBy(s => s.DateOfOrder).ToList();

            //выбираем все вейрхаусы в поставках
            List<WareHouseApi> WareHouseIns = thisOrders.Where(s => s.ActionType.ActionTypeName == "Поступление").SelectMany(w => w.WareHouses).ToList();

            //Назначаем говно для каждого WH 
            foreach (WareHouseApi item in orderItems)
            {
                List<CountChangeHistoryApi> countChangeHistories = new List<CountChangeHistoryApi>();

                //выбираем нужные вейрхаусы которые еще не закончились
                List<WareHouseApi> ThisWareHouseIns = WareHouseIns.Where(
                   s => s.SaleCarId == item.SaleCar.ID &&
                   s.CountRemains > 0).ToList();

                //переводим в массив
                ThisWareHouseIns.ToArray();

                //запоминаем количество которое надо забрать (countRemains - количество которое нам нужно)
                int? countRemains = (int?)item.CountChange;

                //заходим в цикл (пока не возьмем количество которое нам нужно)
                for (int i = 0; countRemains > 0; i++)
                {
                    //запоминаем количество до вычитания
                    int? countRemainsBefore = countRemains;

                    //вычитаем из того сколько нам надо значение остатка первой поставки
                    countRemains -= ThisWareHouseIns[i].CountRemains;

                    //проверяем если количества в поставке хватило
                    if (countRemains <= 0)
                    {
                        //если хватило записываем countRemainsBefore (сколько надо было)
                        countChangeHistories.Add(new CountChangeHistoryApi { Count = countRemainsBefore, WarehouseInId = ThisWareHouseIns[i].ID, WarehouseIn = ThisWareHouseIns[i] });
                    }
                    else
                    {
                        //если нет то ThisWareHouseIns[i].CountRemains (сколько было в поставке)
                        countChangeHistories.Add(new CountChangeHistoryApi { Count = ThisWareHouseIns[i].CountRemains, WarehouseInId = ThisWareHouseIns[i].ID, WarehouseIn = ThisWareHouseIns[i] });
                    }
                    //повторяем
                }
                item.CountChange *= -1;
                item.CountChangeHistories = countChangeHistories;
            }
        }

        /// <summary>
        /// Проверка на добавление в заказ авто
        /// </summary>
        private bool IsCanAddInOrder(SaleCarApi saleCar)
        {
            bool isAdd = false;
            if (saleCar != null)
            {
                List<WareHouseApi> ThisWareHouseIns = Warehouses.Where(s => s.SaleCarId == saleCar.ID).ToList();
                if (ThisWareHouseIns.Sum(s=> s.CountChange) >= saleCar.CountChange)
                {
                    return isAdd = true;
                }
                else
                {
                    return isAdd = false;
                }
            }
            return isAdd;
        }

        public async Task<List<OrderApi>> GetOrder()
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            return Orders;
            //CountChangeHistories = await Api.GetListAsync<List<CountChangeHistoryApi>>("CountChangeHistory");
            //Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            //Warehouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
        }

        public async Task<List<WareHouseApi>> GetWarehouses()
        {
            Warehouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
            return Warehouses;
        }

        public async Task<List<CountChangeHistoryApi>> GetHistory()
        {
            CountChangeHistories = await Api.GetListAsync<List<CountChangeHistoryApi>>("CountChangeHistory");
            return CountChangeHistories;
        }

        public async Task CreateOrder(OrderApi orderApi)
        {
            var order = await Api.PostAsync<OrderApi>(orderApi, "Order");
        }

        public async Task EditOrder(OrderApi orderApi)
        {
            var order = await Api.PutAsync<OrderApi>(orderApi, "Order");
        }

        public async Task GetUser()
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
        }

        private async Task DeleteCar(WareHouseApi wareHouse)
        {
            var warehouse = await Api.DeleteAsync<WareHouseApi>(wareHouse, "Warehouse");
        }

        private async Task DeleteOrder(OrderApi orderApi)
        {
            var order = await Api.DeleteAsync<OrderApi>(orderApi, "Order");
        }

        public async Task CreateWareHouse(WareHouseApi wareHouse)
        {
            var order = await Api.PostAsync<WareHouseApi>(wareHouse, "Warehouse");
        }

        public async Task EditWareHouse(WareHouseApi wareHouse)
        {
            var order = await Api.PutAsync<WareHouseApi>(wareHouse, "Warehouse");
        }
    }
}