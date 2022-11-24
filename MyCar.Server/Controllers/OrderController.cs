using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

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

        private OrderApi CreateOrderApi(Order orderIn, List<CarApi> cars)
        {
            var result = (OrderApi)orderIn;
            result.Cars = cars;
            return result;
        }

        // GET: api/<OrderController>
        [HttpGet]
        public IEnumerable<OrderApi> Get()
        {
            return dbContext.Orders.ToList().Select(s =>
            {
                var warehouses = dbContext.Warehouses.Where(t => t.OrderId == s.Id).Select(t => (CarApi)t.Car).ToList();
                return CreateOrderApi(s, warehouses);
            });
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderApi>> Get(int id)
        {
            var order = await dbContext.Orders.FindAsync(id);
            var products = dbContext.Warehouses.Where(t => t.OrderId == id).Select(t => (CarApi)t.Car).ToList();
            return CreateOrderApi(order, products);
        }

        // POST api/<OrderController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] OrderApi newOrder)
        {
            foreach (var products in newOrder.Cars)
                if (products.ID == 0)
                    return BadRequest($"{products.ModelId} не существует");
            var order = (Order)newOrder;
            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();
            await dbContext.Warehouses.AddRangeAsync(newOrder.WareHouses.Select(s => new Warehouse
            {
                OrderId = order.Id,
                CarId = s.CarId,
                CountChange = s.CountChange,
                Discount = s.Discount,
                Price = s.Price
            }));
            await dbContext.SaveChangesAsync();
            return Ok(order.Id);
        }

        [HttpPut]
        public async Task<ActionResult<OrderApi>> Put(int id, [FromBody] OrderApi editOrder)
        {
            var order = (Order)editOrder;
            var cross = dbContext.Warehouses.FirstOrDefault(s=> s.OrderId == id);
            var oldOrder = await dbContext.Orders.FindAsync(id);
            if(oldOrder == null)
            {
                return NotFound();
            }
            dbContext.Entry(oldOrder).CurrentValues.SetValues(order);
            var warehouseRemove = dbContext.Warehouses.Where(s => s.OrderId == id).ToList();
            dbContext.Warehouses.RemoveRange(warehouseRemove);

            var crosses = editOrder.WareHouses.Select(s => (Warehouse)s);
            await dbContext.Warehouses.AddRangeAsync(crosses.Select(s => new Warehouse
            {
                OrderId = order.Id,
                CarId = s.CarId,
                CountChange = s.CountChange,
                Discount = s.Discount,
                Price = s.Price
            }));
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
