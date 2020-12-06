using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using ClipKeep.Models;

namespace ClipKeep.Controllers
{
    public class MainAppController : Controller
    {
        [Authorize]
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            var userDataModel = new UserData();
            var retrieveUserDataSuccess = userDataModel.GetFromDb();

            // Handle model state edge cases

            if (!retrieveUserDataSuccess || !ModelState.IsValid)
            {
                ViewBag.InfoMessage = null;
                ViewBag.ErrorMessage = "Oh no! Somethings gone wrong whilst getting your data. Try again later.";
            }

            else if (userDataModel.UserClipKeepContents.Count == 0)
            {
                ViewBag.ErrorMessage = null;
                ViewBag.InfoMessage = "Add pasted content to ClipKeep and it will appear here!";
            }

            else
            {
                // Remove/hide both conditional messages as they no longer apply
                ViewBag.ErrorMessage = null;
                ViewBag.InfoMessage = null;
            }
            // Persist the user's data model in the session as we'll access it on post.
            Session["UserDataModel"] = userDataModel;
            return View(userDataModel);
        }

        [Authorize]
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Index(UserData userPaste)
        {
            // The userPaste object is just being used to capture the user's pasted item,
            // what we really want to target is the user data object being persisted in the session.
            var userDataModel = (UserData)Session["UserDataModel"];
            userDataModel.PastedContent = userPaste.PastedContent;

            // Just to ensure MS don't start charging for cosmos usage!
            var maxClipKeepContents = 5;
            var deleteSuccess = true;
            if (userDataModel.UserClipKeepContents.Count == maxClipKeepContents)
            {
                var oldestDocIndex = userDataModel.UserClipKeepContents.Count - 1;
                var docIdToDelete = userDataModel.UserClipKeepContents[oldestDocIndex].DbId;
                deleteSuccess = await userDataModel.DeleteOldestFromDb(docIdToDelete);
            }

            // Don't allow user's to go over storage limits on oldest item deletion error.
            if (!deleteSuccess)
            {
                ViewBag.ErrorMessage = "Oh no! Somethings gone wrong whilst pasting your data. Try again later.";
                return View(userDataModel);
            }

            // In case something's gone wrong with deletion in the past.
            ViewBag.ErrorMessage = null;

            // Store the pasted item in the database
            await userDataModel.StoreInDb();


            return View(userDataModel);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Auth");
        }

    }
}