using DEWAXP.Feature.Builder.Models.LocalPurchase;
using DEWAXP.Foundation.Content.Controllers;
using System.Web.Mvc;

namespace DEWAXP.Feature.Builder.Controllers
{
    public class LocalPurchaseController : BaseController
    {
        public ActionResult Rfx()
        {
            //TODO: Map to a Table
            var response = DewaApiClient.GetOpenRfxInquiries(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage);
            var table = new Table();
            ViewBag.Table = table;
            return PartialView("~/Views/Feature/Builder/LocalPurchase/_Rfx.cshtml");
        }
    }
}