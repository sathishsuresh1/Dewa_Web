using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Helpers.Extensions;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    public class ServiceInterruptionController : BaseController
    {
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult GetServiceInterruptions()
        {
            var serviceResponse = DewaApiClient.GetServiceInterruptionList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

            return View("~/Views/Feature/GeneralServices/ServiceInterruption/_ServiceInterruptions.cshtml", serviceResponse.Payload);
        }
    }
}