using System.Web.Mvc;

namespace DEWAXP.Feature.CommonComponents.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View("~/Views/Feature/CommonComponents/Error/Index.cshtml");
        }
    }
}