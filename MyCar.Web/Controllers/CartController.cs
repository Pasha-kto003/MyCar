using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Web.Core;
using Newtonsoft.Json;
using System.Data;

namespace MyCar.Web.Controllers
{
    public class CartController : Controller
    {
        public List<OrderApi> Orders = new List<OrderApi>();

        public List<SaleCarApi> Cars { get; set; } = new List<SaleCarApi>();


        public List<StatusApi> Statuses { get; set; } = new List<StatusApi>();

        public List<UserApi> Users { get; set; } = new List<UserApi>();

        public List<ActionTypeApi> Types { get; set; }

        public List<WareHouseApi> Warehouses { get; set; } = new List<WareHouseApi>();
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

        public async Task<List<OrderApi>> GetPage(string name)
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            var order = Orders.Where(s => s.User.UserName == name).ToList();
            foreach (var ord in order)
            {
                ord.SumOrder = ord.WareHouses.Sum(s => s.Price);
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
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            Warehouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
            var deleteCar = Warehouses.FirstOrDefault(s => s.ID == id);
            var order = Orders.FirstOrDefault(s => s.ID == deleteCar.OrderId);
            order.WareHouses.Remove(deleteCar);
            await DeleteCar(deleteCar);
            return View("DetailsCart", order);
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
            if (IsCanAddInOrder(car) == false)
            {
                TempData["OrderCountErrorMessage"] = "Превышено максмиальное кол-во покупок данного авто";
                return View("DetailsCarView", car.ID);
            }
            await AddOrder(car.ID, car.CountChange);
            string json = HttpContext.Session.GetString("OrderItem");
            if (json != null)
                orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(json) ?? new List<WareHouseApi>();
            return View("DetailsCart", orderItems);
        }

        public async Task<IActionResult> ConfirmOrder()
        {
            await GetOrders();
            var user = Users.FirstOrDefault(s => s.UserName == User.Identity.Name);
            var actionType = Types.FirstOrDefault(s => s.ActionTypeName == "Продажа");
            var status = Statuses.FirstOrDefault(s => s.StatusName == "Завершен");
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

            await CreateOrder(order);

            orderItems.Clear();
            string json1 = JsonConvert.SerializeObject(orderItems);
            HttpContext.Session.SetString("OrderItem", json1);

            EmailSender emailSender = new EmailSender();
            emailSender.SendEmailAsync(order.User.UserName, order.User.Email, "Пользователь купил авто", "Пользователь купил авто");
            var marks = new List<MarkCarApi>();
            marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            ViewBag.Marks = marks;
            TempData["OrderFineMessage"] = $"Ваш заказ завершен";
            return View("~/Views/Home/Index.cshtml", Cars);
        }
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

        public async Task CreateOrder(OrderApi orderApi)
        {
            var order = await Api.PostAsync<OrderApi>(orderApi, "Order");
        }

        public async Task EditOrder(OrderApi orderApi)
        {
            var order = await Api.PutAsync<OrderApi>(orderApi, "Order");
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