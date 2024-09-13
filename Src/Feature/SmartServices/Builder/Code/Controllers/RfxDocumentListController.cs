using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Helpers.Extensions;
using System.Web.Mvc;

namespace DEWAXP.Feature.Builder.Controllers
{
    public class RfxDocumentListController : BaseController
    {
        // GET: RfxDocumentList
        public PartialViewResult GetRfxList()
        {
            var rfxList = VendorServiceClient.GetOpenRFXInquiries(RequestLanguage, Request.Segment());

            return PartialView("~/Views/Feature/Builder/Tenders/_RfxList.cshtml", rfxList.Payload);
        }
    }
}