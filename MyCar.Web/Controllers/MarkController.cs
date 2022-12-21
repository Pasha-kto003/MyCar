using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsApi;
using MyCar.Web.Core;

namespace MyCar.Web.Controllers
{
    public class MarkController : Controller
    {
        // GET: MarkController
        public ActionResult Index()
        {
            return View();
        }

        // GET: MarkController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MarkController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MarkController/Create
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

        // GET: MarkController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MarkController/Edit/5
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
    }
}
