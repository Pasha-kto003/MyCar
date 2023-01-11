using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Web.Core;

namespace MyCar.Web.Controllers
{
    public class CarController : Controller
    {
        public List<CarApi> Cars { get; set; } = new List<CarApi>();
        // GET: CarController
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult CarView()
        {
            return View();
        }


        // GET: CarController/Details/5
        [Route("/Car/DetailsCarView/CarName/{CarName?}")]
        public async Task<IActionResult> DetailsCarView(string CarName)
        {
            Cars = await Api.GetListAsync<List<CarApi>>("Car");
            var car = Cars.FirstOrDefault(s => s.CarName == CarName);
            return View(car);
        }

        // GET: CarController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CarController/Create
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

        // GET: CarController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CarController/Edit/5
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

        // GET: CarController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CarController/Delete/5
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
