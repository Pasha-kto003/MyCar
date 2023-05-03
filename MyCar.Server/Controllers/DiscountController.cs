using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DataModels;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        // GET: api/<DiscountController>
        private readonly MyCar_DBContext dbContext;
        public DiscountController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<DiscountApi> Get()
        {
            return dbContext.Discounts.ToList().Select(s =>
            {
                return GetDiscount(s);
            });
        }

        private DiscountApi GetDiscount(Discount discount)
        {
            return ModelData.GetDiscount(discount, dbContext);
        }

        // GET api/<DiscountController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DiscountApi>> Get(int id)
        {
            var discount = await dbContext.Discounts.FindAsync(id);
            if (discount == null)
                return NotFound();
            return Ok((DiscountApi)discount);
        }

        // POST api/<DiscountController>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] DiscountApi discountApi)
        {
            var newDiscount = (Discount)discountApi;
            await dbContext.Discounts.AddAsync(newDiscount);
            await dbContext.SaveChangesAsync();
            return Ok(newDiscount.Id);
        }

        // PUT api/<DiscountController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] DiscountApi discountApi)
        {
            var result = await dbContext.Discounts.FindAsync(id);
            if (result == null)
                return NotFound();
            var discount = (Discount)discountApi;
            dbContext.Entry(result).CurrentValues.SetValues(discount);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<DiscountController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var discount = await dbContext.Discounts.FindAsync(id);
            if (discount == null)
                return NotFound();
            dbContext.Discounts.Remove(discount);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
