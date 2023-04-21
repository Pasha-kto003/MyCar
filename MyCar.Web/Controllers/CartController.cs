using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Web.Core;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Web;

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

        public async Task<IActionResult> AddOrder(int id)
        {
            await GetOrders();
            List<WareHouseApi> orderItems = new List<WareHouseApi>();
            var car = Cars.FirstOrDefault(s => s.ID == id);
            var order = Orders.LastOrDefault(s => s.Status.StatusName == "Завершен");
            //var warehouses = Warehouses.Where(s => s.OrderId == null || s.OrderId == 0).ToList();
            if(order != null)
            {
                WareHouseApi warehouse = new WareHouseApi
                {
                    SaleCarId = id,
                    CountChange = -1,
                    Discount = 0,
                    SaleCar = car,
                    Price = car.FullPrice
                };

                string json = JsonConvert.SerializeObject(warehouse);
                Response.Cookies.Append("OrderItem", json, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1) // устанавливаем время жизни куки на 1 день
                });
                Request.Cookies.TryGetValue("OrderItem", out string answer);
                if (!string.IsNullOrEmpty(answer))
                {
                    orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(answer);
                }
            }
            return View("DetailsCart", orderItems);
        }

        public async Task<IActionResult> AddCars(int id)
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            var order = Orders.FirstOrDefault(s => s.ID == id);
            return View("DetailsCart", order);
        }

        public async Task<IActionResult> DetailsCart(string name)
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            Warehouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
            Users = await Api.GetListAsync<List<UserApi>>("User");
            var user = Users.FirstOrDefault(s => s.UserName == name);
            var order = Orders.LastOrDefault(s => s.User.UserName == name && s.Status.StatusName == "Новый");
            if (order == null)
            {
                return View("Error");
            }
            else if(order.WareHouses == null || order.WareHouses.Count == 0)
            {
                DeleteOrder(order);
                return View("Error");
            }
            order.WareHouses = Warehouses.Where(s => s.OrderId == order.ID).ToList();
            return View("DetailsCart", order);
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

        public async Task<IActionResult> ConfirmOrder()
        {
            await GetOrders();
            var user = Users.FirstOrDefault(s=> s.UserName == User.Identity.Name);
            var warehouses = Warehouses.Where(s => s.OrderId == null || s.OrderId == 0).ToList();
            var actionType = Types.FirstOrDefault(s => s.ID == 3);
            var status = Statuses.FirstOrDefault(s => s.ID == 2);
            //List<WareHouseApi> orderItems = new List<WareHouseApi>();
            //Request.Cookies.TryGetValue("OrderItem", out string answer);
            //if (!string.IsNullOrEmpty(answer))
            //{
            //    orderItems = JsonConvert.DeserializeObject<List<WareHouseApi>>(answer);
            //}
            OrderApi order = new OrderApi
            {
                UserId = user.ID,
                DateOfOrder = DateTime.Now,
                ActionTypeId = 3,
                StatusId = 2,
                WareHouses = warehouses,
                ActionType = actionType,
                Status = status,
                User = user
            };
            await CreateOrder(order);


            return View("DetailsCart", warehouses);
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
