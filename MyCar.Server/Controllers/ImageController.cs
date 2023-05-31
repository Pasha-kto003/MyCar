using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Server.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCar.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly MyCar_DBContext dbContext;
        public ImageController(MyCar_DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("images/{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            var imagePath = Path.Combine(AppContext.BaseDirectory, "Images", imageName);
            var defaultPicture = Path.Combine(AppContext.BaseDirectory, "Images", "picture.png");
            var imageBytes = new byte[0];
            try
            {
                if (System.IO.File.Exists(imagePath))
                {
                    imageBytes = System.IO.File.ReadAllBytes(imagePath);
                }
                else
                {
                    imageBytes = System.IO.File.ReadAllBytes(defaultPicture);
                }
                return File(imageBytes, "image/png");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при обработке файла: " + ex.Message);
                return BadRequest("No file uploaded.");
            }
        }

        [HttpPost("images")]
        public IActionResult SaveImage([FromForm] IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    var filePath = Path.Combine(AppContext.BaseDirectory, "Images", file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    return Ok(file.FileName); // Опционально, возвращает имя сохраненного файла
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка при обработке файла: " + ex.Message);
                    return BadRequest("No file uploaded.");
                }
            }
            return BadRequest("No file uploaded.");
        }
    }
}
