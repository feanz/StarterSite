using System.Web.Mvc;
using StarterSite.Web.Helpers;

namespace StarterSite.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("Home")]
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";

            return View();
        }

        [Route("About")]
        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }
    }
}
