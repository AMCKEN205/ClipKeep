using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClipKeep.Controllers
{
    [Authorize]
    public class MainAppController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Settings()
        {
            ViewBag.Message = "Your settings page.";

            return View();
        }
    }
}