using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Web.Core;
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
        public ActionResult Details(int id)
        {
            return View();
        }

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
        public async Task<IActionResult> CartPage(string name)
        {
            name = User.Identity.Name;
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            var order = Orders.Where(s=> s.User.UserName == name);
            return View(order);
        }

        public async Task<IActionResult> AddOrder(int id)
        {
            Cars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            Warehouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
            Users = await Api.GetListAsync<List<UserApi>>("User");
            Statuses = await Api.GetListAsync<List<StatusApi>>("Status");
            Types = await Api.GetListAsync<List<ActionTypeApi>>("ActionType");
            var car = Cars.FirstOrDefault(s=> s.ID == id);
            WareHouseApi wareHouse = new WareHouseApi();
            wareHouse.ID = Warehouses.Count() + 1;
            wareHouse.SaleCar = car;
            wareHouse.SaleCarId = car.ID;
            wareHouse.CountChange = -1;
            wareHouse.Discount = 0;
            wareHouse.Price = car.FullPrice;
            OrderApi order = new OrderApi();
            order.WareHouses = new List<WareHouseApi>();
            order.WareHouses.Add(wareHouse);
            order.SumOrder = car.FullPrice - wareHouse.Discount;
            var user = Users.FirstOrDefault(s => s.UserName == User.Identity.Name);
            order.User = user;
            order.UserId = user.ID;
            order.DateOfOrder = DateTime.Now;
            order.StatusId = 1;
            order.Status = Statuses.FirstOrDefault(s => s.ID == order.StatusId);
            order.ActionTypeId = 2;
            order.ActionType = Types.FirstOrDefault(s => s.ID == order.ActionTypeId);
            await CreateOrder(order);
            wareHouse.OrderId = order.ID;
            await CreateWareHouse(wareHouse);
            return View("DetailsCart", order);
        }



        public async Task CreateOrder(OrderApi orderApi)
        {
            var order = await Api.PostAsync<OrderApi>(orderApi, "Order");
        }

        public async Task CreateWareHouse(WareHouseApi wareHouse)
        {
            var order = await Api.PostAsync<WareHouseApi>(wareHouse, "Warehouse");
        }

        // GET: CartController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CartController/Edit/5
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

        // GET: CartController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CartController/Delete/5
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
