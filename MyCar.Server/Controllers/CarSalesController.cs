using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DataModels;
using MyCar.Server.DB;
using System.Text.Json;

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
            return ModelData.SaleGet(saleCar, dbContext);
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
                    case "Авто":
                        SalesApi = SalesApi.Where(s => s.Car.CarName.ToLower().Contains(text)).ToList();
                        break;
                    case "Модель":
                        SalesApi = SalesApi.Where(s => s.Car.Model.ModelName.ToLower().Contains(text)).ToList();
                        break;
                    case "Марка":
                        SalesApi = SalesApi.Where(s => s.Car.Model.MarkCar.MarkName.ToLower().Contains(text)).ToList();
                        break;
                    case "Цена авто":
                        SalesApi = SalesApi.Where(s => s.FullPrice.ToString().ToLower().Contains(text)).ToList();
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
