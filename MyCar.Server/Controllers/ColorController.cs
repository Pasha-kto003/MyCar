using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using System.Text.Json;

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        // GET: ColorController
        [HttpGet]
        public IEnumerable<Color> Get()
        {
            List<Color> colors = new List<Color>();
            using (FileStream fs = new FileStream("./DataModels/ColorCar/colors.json", FileMode.OpenOrCreate))
            {
                colors = JsonSerializer.Deserialize<List<Color>>(fs) ?? new List<Color>();
            }
            return colors;

        }
    }
}
