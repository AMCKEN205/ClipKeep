using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ClipKeep.Models;

namespace ClipKeep.Controllers
{
    public class AuthController : Controller
    {

        /// <summary>
        /// Styled colouring for subtext 'ClipKeep' text.
        /// </summary>
        private readonly string _clipKeepTextStyled =
            "<b><span style=\"color: #7483f1\">Clip</span><span style=\"color: #fdba12\">Keep</span></b>";

        // Login 

        /// <summary>
        /// Handle user login view startup
        /// </summary>
        /// <returns> The login view connected to the user model </returns>
        public ActionResult Login()
        {

            ViewBag.SubText = $"Login to {_clipKeepTextStyled}";
            var userModel = new User();
            return View(userModel);
        }

        /// <summary>
        /// Handle a user submitting login credentials
        /// </summary>
        [HttpPost]
        public ActionResult Login(User userModel)
        {
            FormsAuthentication.SetAuthCookie(userModel.UserId, false);
            return RedirectToAction("Index");
        }

        // Register

        /// <summary>
        /// Handle user register view startup
        /// </summary>
        /// <returns> The login view connected to the user model </returns>
        public ActionResult Register()
        {
            // Styled colouring for subtext 'ClipKeep' text.
            string clipKeepTextStyled =
                "<b><span style=\"color: #7483f1\">Clip</span><span style=\"color: #fdba12\">Keep</span></b>";
            ViewBag.SubText = $"Login to {clipKeepTextStyled}";
            var userModel = new User();
            return View(userModel);
        }

        /// <summary>
        /// Handle a user submitting details for registration
        /// </summary>
        [HttpPost]
        public ActionResult Register(User userModel)
        {
            FormsAuthentication.SetAuthCookie(userModel.UserId, false);
            return RedirectToAction("Index");
        }
    }
}