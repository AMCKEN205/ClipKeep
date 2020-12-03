using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpGet]
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
        public async  Task<ActionResult> Login(User userModel)
        {
            // Handle misentry of model attributes first (e.g. no email entered).
            if (!ModelState.IsValid)
            {
                // And return the original view with the model currently under use
                return View(userModel);
            }
            FormsAuthentication.SetAuthCookie(userModel.Username, false);
            return RedirectToAction("Index");
        }

        // Register

        /// <summary>
        /// Handle user register view startup
        /// </summary>
        /// <returns> The login view connected to the user model </returns>
        [HttpGet]
        public ActionResult Register()
        {
            // Styled colouring for subtext 'ClipKeep' text.
            ViewBag.SubText = $"Sign up to {_clipKeepTextStyled}";
            var userModel = new UserRegister();
            return View(userModel);
        }

        /// <summary>
        /// Handle a user submitting details for registration
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Register(UserRegister userRegisterModel)
        {
            // Handle misentry of model attributes first (e.g. no username entered).
            if (!ModelState.IsValid)
            {
                // And return the original view with the model currently under use
                return View(userRegisterModel);
            }

            // Add the newly registered user to the DB
            var userAddSuccess = await userRegisterModel.StoreInDb();

            // Return specific view based on the success or failure of adding a user.
            if (userAddSuccess)
            {
                ViewBag.FailToRegisterError = null;
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.FailToRegisterError = "Sorry, somethings gone wrong on our end! Try again later.";
                return View(userRegisterModel);
            }
        }
    }
}