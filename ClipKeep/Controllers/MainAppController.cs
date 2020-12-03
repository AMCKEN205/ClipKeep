using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClipKeep.Models;

namespace ClipKeep.Controllers
{
    public class MainAppController : Controller
    {

        public ActionResult Index()
        {
            var userModel = GetUser();
            // Check we're dealing with an authorised user first.
            if (userModel.Username == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        public ActionResult Settings(User userModel = null)
        {
            // Check we're dealing with an authorised user first.
            if (userModel.Username == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Message = "Settings page.";

            return View();
        }

        private User GetUser()
        {
            if (HttpContext.Session["User"] == null)
            {
                // TODO: User load from cookie code
                HttpContext.Session["User"] = new User();
            }

            return HttpContext.Session["User"] as User;
        }

    }
}