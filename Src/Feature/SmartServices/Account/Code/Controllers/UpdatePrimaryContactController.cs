using DEWAXP.Feature.Account.Models.UpdatePrimaryContact;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Helpers.Extensions;
using System;
using System.Web.Mvc;

namespace DEWAXP.Feature.Account.Controllers
{
    [TwoPhaseAuthorize]
    public class UpdatePrimaryContactController : BaseController
    {
        [HttpGet]
        public ActionResult NewRequest()
        {
            try
            {
                UpdatePrimaryContactModel model = null;
                model = ContextRepository.GetCurrentItem<UpdatePrimaryContactModel>();
                return PartialView("~/Views/Feature/Account/UpdatePrimaryContact/_NewRequest.cshtml", model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewRequest(UpdatePrimaryContactModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string error = string.Empty;

                    var response = DewaApiClient.UpdatePrimaryContactDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, model.EmailAddress, model.MobileNumber.AddMobileNumberZeroPrefix(), RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.UPDATE_PRIMARY_CONTACT_SUCCESS);

                    ModelState.AddModelError(string.Empty, error);
                }
                return PartialView("~/Views/Feature/Account/UpdatePrimaryContact/_NewRequest.cshtml", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return null;
            }
        }

        [HttpGet]
        public ActionResult RequestSent()
        {
            return PartialView("~/Views/Feature/Account/UpdatePrimaryContact/_RequestSent.cshtml");
        }
    }
}