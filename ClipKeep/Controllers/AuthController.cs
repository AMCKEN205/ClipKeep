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
        [AllowAnonymous]
        public ActionResult Login()
        {
            var userIsAuthenticated = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (userIsAuthenticated)
            {
                return RedirectToAction("Index", "MainApp");
            }

            ViewBag.SubText = $"Login to {_clipKeepTextStyled}";
            var userModel = new UserLogin();
            return View(userModel);
        }

        /// <summary>
        /// Handle a user submitting login credentials
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserLogin userLoginModel)
        {
            // Handle misentry of model attributes first (e.g. no email entered).
            if (!ModelState.IsValid)
            {
                // And return the original view with the model currently under use
                return View(userLoginModel);
            }

            // In retrospect it would have been better to use more of the inbuilt form authentication
            // db methods (e.g. use login builtin instead of just diving in and doing myself)
            // instead of validating against my own cosmos DB in code.
            // However, couldn't find a method for integrating with cosmos DB.
            FormsAuthentication.SetAuthCookie(userLoginModel.Username, false);
            return RedirectToAction("Index", "MainApp");


        }

        // Register

        /// <summary>
        /// Handle user register view startup
        /// </summary>
        /// <returns> The login view connected to the user model </returns>
        [AllowAnonymous]
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
        [AllowAnonymous]
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