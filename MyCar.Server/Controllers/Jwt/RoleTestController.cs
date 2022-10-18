using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

namespace MyCar.Server.Controllers.Jwt
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleTestController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public RoleTestController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet(Name = "GetCars"), Authorize(Roles = "Admin")]
        public IEnumerable<CarApi> Get()
        {
            return dbContext.Cars.Select(s => (CarApi)s);
        }
    }
}
