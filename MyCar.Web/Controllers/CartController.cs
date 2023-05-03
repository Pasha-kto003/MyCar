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

        public List<UserApi> Users { get; set; } = new List<UserApi>();

        public List<ActionTypeApi> Types { get; set; }

        public List<StatusApi> Statuses { get; set; }

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

        public async Task<IActionResult> AddOrder(int id)
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
            var order = Orders.LastOrDefault(s => s.Status.StatusName == "Завершен" && s.UserId == user.ID);
            //var warehouses = Warehouses.Where(s => s.OrderId == null || s.OrderId == 0).ToList();
            List<OrderApi> thisOrders = Orders.OrderBy(s => s.DateOfOrder).ToList();
            List<WareHouseApi> WareHouseIns = thisOrders.Where(s => s.ActionType.ActionTypeName == "Поступление").SelectMany(w => w.WareHouses).ToList();
            if (order != null)
            {
                WareHouseApi warehouse = new WareHouseApi
                {
                    SaleCarId = id,
                    CountChange = 1,
                    Discount = 0,
                    SaleCar = car,
                    Price = price
                };
                // Получаем текущий список из Session
                string json = HttpContext.Session.GetString("OrderItem");
                if (json != null)
                    orderItemsOld = JsonConvert.DeserializeObject<List<WareHouseApi>>(json) ?? new List<WareHouseApi>();

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
            }
            return View("DetailsCart", orderItems);
        }

        public async Task<IActionResult> ConfirmOrder()
        {
            await GetOrders();
            var user = Users.FirstOrDefault(s => s.UserName == User.Identity.Name);
            var warehouses = Warehouses.Where(s => s.OrderId == null || s.OrderId == 0).ToList();
            var actionType = Types.FirstOrDefault(s => s.ActionTypeName == "Продажа");
            var status = Statuses.FirstOrDefault(s => s.StatusName == "Завершен");
            List<WareHouseApi> orderItems = new List<WareHouseApi>();

            Request.Cookies.TryGetValue("OrderItem", out string answer);

            string json = HttpContext.Session.GetString("OrderItem");
            if (json != null)
                orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(json) ?? new List<WareHouseApi>();

            OrderApi order = new OrderApi
            {
                UserId = user.ID,
                DateOfOrder = DateTime.Now,
                ActionTypeId = 2,
                StatusId = 2,
                WareHouses = orderItems,
                ActionType = actionType,
                Status = status,
                User = user
            };
            await CreateOrder(order);
            EmailSender emailSender = new EmailSender();
            emailSender.SendEmailAsync(order.User.UserName, order.User.Email, "Пользователь купил авто", "Пользователь купил авто");
            return View("SuccsessOrder");
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