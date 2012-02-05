using System.Web.Mvc;
using StarterSite.Web.Helpers;

namespace StarterSite.Web.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        /// This is fired when the site hits a 500
        /// </summary>
        /// <returns></returns>
        [Route("Error")]
        public ActionResult Problem()
        {
            return View();
        }

        /// <summary>
        /// This is fired when the site gets a bad URL
        /// </summary>
        /// <returns></returns>        
        public ActionResult NotFound(string aspxerrorpath)
        {
            TempData["ErrorPath"] = aspxerrorpath;
            return View();
        }

        [Route("NoAccess")]
        public ActionResult NoAccess()
        {
            return View();
        }

        [Route("InsufficientPermissions")]
        public ActionResult InsufficientPermissions()
        {
            return View();
        }
    }
}
