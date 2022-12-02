using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarSalesController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public CarSalesController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/<CarSalesController>
        private SaleCarApi GetSale(SaleCar saleCar)
        {
            var result = (SaleCarApi)saleCar;
            var equipment = dbContext.Equipment.FirstOrDefault(s => s.Id == saleCar.EquipmentId);
            result.Equipment = (EquipmentApi)equipment;
            var car = dbContext.Cars.FirstOrDefault(s => s.Id == saleCar.CarId);
            result.Car = (CarApi)car;
            var model = dbContext.Models.FirstOrDefault(s=> s.Id == car.ModelId);
            result.Car.Model = (ModelApi)model;
            var mark = dbContext.MarkCars.FirstOrDefault(s => s.Id == model.MarkId);
            result.Car.CarMark = mark.MarkName;
            return result;
        }

        // GET: api/<SaleCarController>
        [HttpGet]
        public IEnumerable<SaleCarApi> GetSales()
        {
            return dbContext.SaleCars.ToList().Select(s =>
            {
                return GetSale(s);
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleCarApi>> Get(int id)
        {
            var sale = await dbContext.SaleCars.FindAsync(id);
            return GetSale(sale);
        }

        [HttpGet("Type, Text, Filter")]
        public IEnumerable<SaleCarApi> SearchSale(string type, string text, string filter)
        {
            IEnumerable<SaleCarApi> SalesApi = dbContext.SaleCars.ToList().Select(s =>
            {
                return GetSale(s);
            });

            if(text != "$")
                switch (type)
                {
                    case "Артикул":
                        SalesApi = SalesApi.Where(s => s.Articul.ToLower().Contains(text)).ToList();
                        break;
                    case "Машина":
                        SalesApi = SalesApi.Where(s => s.Car.Model.ModelName.ToLower().Contains(text)).ToList();
                        break;
                    case "Цена комплектации":
                        SalesApi = SalesApi.Where(s => s.EquipmentPrice.ToString().ToLower().Contains(text)).ToList();
                        break;
                    default:
                        SalesApi = SalesApi.ToList();
                        break;
                }
                if(filter != "Все")
                {
                    int id = dbContext.Equipment.FirstOrDefault(s => s.NameEquipment.ToLower().Equals(filter)).Id;
                    SalesApi = SalesApi.Where(s=> s.EquipmentId == id);
                }

            return SalesApi.ToList();
        }

        // POST api/<CarSalesController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] SaleCarApi saleApi)
        {
            var sale = (SaleCar)saleApi;
            await dbContext.SaleCars.AddAsync(sale);
            await dbContext.SaveChangesAsync();
            return Ok(sale.Id);
        }

        // PUT api/<CarSalesController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] SaleCarApi saleCarApi)
        {
            var result = await dbContext.SaleCars.FindAsync(id);
            if(result == null)
                return NotFound();
            var sale = (SaleCar)saleCarApi;
            dbContext.Entry(result).CurrentValues.SetValues(sale);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<CarSalesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var sale = await dbContext.SaleCars.FindAsync(id);
            if (sale == null)
                return NotFound();
            dbContext.Remove(sale);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
