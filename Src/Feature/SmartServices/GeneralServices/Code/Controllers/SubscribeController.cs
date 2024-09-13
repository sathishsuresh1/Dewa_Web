using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    [TwoPhaseAuthorize]
    public class SubscribeController : BaseController
    {
        [HttpGet]
        public ActionResult Subscribe()
        {
            var emailListResponse = DewaApiClient.GetEmailListForMobileNumber(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);

            if (emailListResponse.Succeeded)
            {
                var model = emailListResponse.Payload.ToList();
                model.RemoveAll(c => string.IsNullOrEmpty(c.Email));
                return PartialView("~/Views/Feature/GeneralServices/Subscribe/_Subscribe.cshtml", model);
            }
            ViewBag.Message = emailListResponse.Message;
            return PartialView("~/Views/Feature/GeneralServices/Subscribe/_SubscribeError.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Subscribe(string emailsList, string settingsList)
        {
            emailsList = emailsList.Replace(",", ";");
            settingsList = settingsList.Replace(",", ";");

            var subscribeResponse = DewaApiClient.SetMarketingPreferenceEmail(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, emailsList, settingsList);

            if (subscribeResponse.Succeeded)
            {
                return PartialView("~/Views/Feature/GeneralServices/Subscribe/_SubscribeConfirmation.cshtml");
            }
            ViewBag.Message = subscribeResponse.Message;
            return PartialView("~/Views/Feature/GeneralServices/Subscribe/_SubscribeError.cshtml");
        }
    }
}