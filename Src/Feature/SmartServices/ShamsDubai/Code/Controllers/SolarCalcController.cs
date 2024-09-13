using DEWAXP.Feature.ShamsDubai.Models.SolarCalculator;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Shell.Framework.Commands.UserManager;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.ShamsDubai.Controllers
{
    public class SolarCalcController : BaseController
    {
        [AcceptVerbs("GET", "HEAD")]
        public ActionResult NewRequest()
        {
            var model = ContextRepository.GetCurrentItem<SolarCalculatorModel>();
            //model.SessionID = CurrentPrincipal.SessionToken;
            return PartialView("~/Views/Feature/ShamsDubai/SolarCalculator/_NewRequest.cshtml", model);
        }

        [HttpGet]
        public ActionResult SolarCalculatorAccounts()
        {
            if (String.IsNullOrEmpty(CurrentPrincipal.SessionToken))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SOLAR_CALCULATOR_MAP);
            }
            return PartialView("~/Views/Feature/ShamsDubai/SolarCalculator/_SelectAccount.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult SelectAccount(SelectedAccount selectedAccount)
        {
            //var accountFromService = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment()).Payload
            //    .FirstOrDefault(x => x.AccountNumber == selectedAccount.SelectedAccountNumber);

            var accountFromService = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment()).Payload
                .FirstOrDefault(x => x.AccountNumber == selectedAccount.SelectedAccountNumber);

            var account = SharedAccount.CreateFrom(accountFromService);

            if (!ModelState.IsValid)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J18_MOVE_OUT_START);
            }
            CacheProvider.Store(CacheKeys.SOLAR_CALCULATOR_SELECTED_ACCOUNT, new CacheItem<SharedAccount>(account));

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SOLAR_CALCULATOR_MAP);
        }
    }
}