using DEWAXP.Feature.GeneralServices.TermsAndConditions;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using System;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    public class TermsController : BaseController
    {
        [HttpGet]
        public ActionResult SetTermsAndConditions()
        {
            if (!IsLoggedIn)
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

            var model = new Terms();
            model = GetMailTo(model);
            return View("~/Views/Feature/GeneralServices/TermsAndConditions/_Terms.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetTermsAndConditions(Terms model)
        {
            var response = DewaApiClient.SetPrimaryAccount(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, General.AcceptedTerms, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                DewaProfile profile = AuthStateService.GetActiveProfile();
                profile.TermsAndConditions = General.AcceptedTerms;
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View("~/Views/Feature/GeneralServices/TermsAndConditions/_Terms.cshtml");
            }
        }

        public Terms GetMailTo(Terms model)
        {
            var itemUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.TERMS_AND_CONDITIONS, false);
            var item = ContentRepository.GetItem<FormattedText>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.TERMS_AND_CONDITIONS_TEXT_PLAIN)));
            if (item != null)
            {
                var emailBody = item.RichText.Replace("PAGE_LINK", itemUrl);

                model.MailSubject = item.Name.Replace("Plain", "");
                model.MailBody = emailBody;
            }
            return model;
        }
    }
}