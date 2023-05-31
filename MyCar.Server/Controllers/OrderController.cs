using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;
using MyCar.Server.DataModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public OrderController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private OrderApi CreateOrderApi(Order orderIn)
        {
            return ModelData.OrderGet(orderIn, dbContext);
        }

        // GET: api/<OrderController>
        [HttpGet]
        public IEnumerable<OrderApi> Get()
        {
            return dbContext.Orders.ToList().Select(s =>
            {
                return CreateOrderApi(s);
            });
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderApi>> Get(int id)
        {
            var order = await dbContext.Orders.FindAsync(id);
            return CreateOrderApi(order);
        }

        [HttpGet("Type, Text, Filter")]
        public IEnumerable<OrderApi> SearchOrder(string type, string text, string filter)
        {
            IEnumerable<OrderApi> OrdersApi = dbContext.Orders.ToList().Select(s =>
            {
                return CreateOrderApi(s);
            });

            if (text != "$")
            {
                switch (type)
                {
                    case "Заказчик":
                        OrdersApi = OrdersApi.Where(s => s.User.UserName.ToLower().Contains(text)).ToList();
                        break;
                    case "Дата":
                        OrdersApi = OrdersApi.Where(s => s.DateOfOrder.ToString().ToLower().Contains(text)).ToList();
                        break;
                    case "№ Заказа":
                        OrdersApi = OrdersApi.Where(s => s.ID.ToString().ToLower().Contains(text)).ToList();
                        break;
                    default:
                        OrdersApi = OrdersApi.ToList();
                        break;
                }
            }

            if(filter != "Все")
            {
                int id = dbContext.ActionTypes.FirstOrDefault(s => s.ActionTypeName.ToLower().Contains(filter)).Id;
                OrdersApi = OrdersApi.Where(s => s.ActionTypeId == id);
            }

            return OrdersApi.ToList();
        }

        // POST api/<OrderController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] OrderApi newOrder)
        {
            var oldCount = dbContext.Orders.Count();
            var order = (Order)newOrder;
            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();
            var warehouses = newOrder.WareHouses.Select(s => new Warehouse
            {
                OrderId = order.Id,
                SaleCarId = s.SaleCarId,
                CountChange = s.CountChange,
                Discount = s.Discount,
                Price = s.Price
            });
            await dbContext.Warehouses.AddRangeAsync(warehouses);
            await dbContext.SaveChangesAsync();

            if (newOrder.ActionType.ActionTypeName != "Поступление")
            {

                List<CountChangeHistory> countChangeHistories = new List<CountChangeHistory>();
                foreach (var wh in dbContext.Orders.First(s=>s.Id == order.Id).Warehouses)
                {
                    WareHouseApi OldWare = newOrder.WareHouses.First(s => s.SaleCarId == wh.SaleCarId);
                    foreach (var cch in OldWare.CountChangeHistories)
                    {
                        CountChangeHistory countChangeHis = new CountChangeHistory
                        {
                            Count = cch.Count,
                            WarehouseInId = cch.WarehouseInId,
                            WarehouseOutId = wh.Id,
                        };
                        countChangeHistories.Add(countChangeHis);
                    }
                }

                await dbContext.CountChangeHistories.AddRangeAsync(countChangeHistories);
                await dbContext.SaveChangesAsync();
            }

            return Ok(order.Id);
        }

        // PUT api/<OrderController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderApi>> Put(int id, [FromBody] OrderApi editOrder)
        {
            var order = (Order)editOrder;
            //var cross = dbContext.Warehouses.FirstOrDefault(s=> s.OrderId == id);
            var oldOrder = await dbContext.Orders.FindAsync(id);
            order.Status = dbContext.Statuses.FirstOrDefault(s=> s.Id == editOrder.StatusId);
            if(oldOrder == null)
            {
                return NotFound();
            }
            dbContext.Entry(oldOrder).CurrentValues.SetValues(order);
            await dbContext.SaveChangesAsync();

            //По логике код снизу не нужен, так как состав заказа изменять нельзя (только отменить и создать новый)(если все же, код ниже где то используется, то он не работает)

            //var warehouseRemove = dbContext.Warehouses.Where(s => s.OrderId == id).ToList();
            //dbContext.Warehouses.RemoveRange(warehouseRemove);

            //var crosses = editOrder.WareHouses.Select(s => (Warehouse)s);
            //await dbContext.Warehouses.AddRangeAsync(crosses.Select(s => new Warehouse
            //{
            //    OrderId = order.Id,
            //    SaleCarId = s.SaleCarId,
            //    CountChange = s.CountChange,
            //    Discount = s.Discount,
            //    Price = s.Price
            //}));
            //await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
