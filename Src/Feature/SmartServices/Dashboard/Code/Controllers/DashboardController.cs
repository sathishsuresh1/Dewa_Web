using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Feature.Dashboard.Models;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Bills;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
using System;
using System.Web.Mvc;

namespace DEWAXP.Feature.Dashboard.Controllers
{
    [TwoPhaseAuthorize(AllowEVUsers = true)]
    public class DashboardController : BaseController
    {
        [HttpGet]
        public PartialViewResult Home()
        {
            return PartialView("~/Views/Feature/Dashboard/Dashboard/_Home.cshtml");
        }

        [AcceptVerbs("GET", "HEAD")]
        public PartialViewResult Home_bes()
        {
            return PartialView("~/Views/Feature/Dashboard/Dashboard/_Home_bes.cshtml");
        }

        [HttpGet]
        public PartialViewResult Consumption()
        {
            Session["Isvisitedconsumption"] = "true";
            return PartialView("~/Views/Feature/Dashboard/Dashboard/_Consumption.cshtml");
        }

        [HttpGet]
        public PartialViewResult ConsumptionComparison()
        {
            return PartialView("~/Views/Feature/Dashboard/Dashboard/_ConsumptionComparison.cshtml");
        }

        [HttpGet]
        public PartialViewResult UserWidget()
        {
            return PartialView("~/Views/Feature/Dashboard/Dashboard/_UserWidget.cshtml", new UserModel
            {
                Name = CurrentPrincipal.Username,
                Username = CurrentPrincipal.Username,
                SessionToken = CurrentPrincipal.SessionToken,
                EmailAddress = CurrentPrincipal.EmailAddress,
                MobileNumber = CurrentPrincipal.MobileNumber,
                UserHasProfilePhoto = CurrentUserProfilePhoto.HasProfilePhoto,
                UserProfilePhoto = CurrentUserProfilePhoto.ProfilePhoto
            });
        }

        [HttpGet]
        public PartialViewResult HomeV1()
        {
            return PartialView("~/Views/Feature/Dashboard/Dashboard/_HomeV1.cshtml");
        }

        [HttpGet]
        public ActionResult DashboardHeader()
        {
            return View("~/Views/Feature/Dashboard/Dashboard/DashboardHeader.cshtml");
        }

        [HttpGet]
        public PartialViewResult HomeV2()
        {
            return PartialView("~/Views/Feature/Dashboard/Dashboard/_HomeV2.cshtml");
        }

        [HttpGet]
        public ActionResult ConsumptionDetails()
        {
            return View("~/Views/Feature/Dashboard/Dashboard/_ConsumptionDetails.cshtml");
        }

        [HttpGet]
        public ActionResult BillCompareComponent()
        {
            return View("~/Views/Feature/Dashboard/Dashboard/BillCompareComponent.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BillCompare(string account)
        {
            if (!string.IsNullOrWhiteSpace(account))
            {
                AccountCache transactionaccount;
                BillHistoryResponse paymentHistoryDetail = null;
                BillCompareViewModel billcompareModel = new BillCompareViewModel();

                if (CacheProvider.TryGet(CacheKeys.SELECTED_TRANSACTIONACCOUNT, out transactionaccount))
                {
                    if (transactionaccount != null && transactionaccount.accountnumber.Equals(account) && transactionaccount.RequestLanguage.Equals(RequestLanguage))
                    {
                        if (CacheProvider.TryGet(CacheKeys.SELECTED_TRANSACTION, out paymentHistoryDetail))
                        {
                        }
                    }
                }
                if (paymentHistoryDetail == null)
                {
                    var response = SmartCustomerClient.GetBillPaymentHistory(
                        new BillHistoryRequest
                        {
                            contractaccountnumber = account,
                            sessionid = CurrentPrincipal.SessionToken,
                            userid = CurrentPrincipal.UserId
                        }, RequestLanguage, Request.Segment());
                    // DewaApiClient.GetPaymentHistory(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, account, RequestLanguage, Request.Segment());
                    if (response != null && response.Succeeded && response.Payload != null)
                    {
                        CacheProvider.Store(CacheKeys.SELECTED_TRANSACTION, new CacheItem<BillHistoryResponse>(response.Payload, TimeSpan.FromMinutes(20)));
                        CacheProvider.Store(CacheKeys.SELECTED_TRANSACTIONACCOUNT, new CacheItem<AccountCache>(new AccountCache { accountnumber = account, RequestLanguage = RequestLanguage }, TimeSpan.FromMinutes(20)));
                        paymentHistoryDetail = response.Payload;
                    }
                }
                billcompareModel = BillCompareViewModel.From(paymentHistoryDetail);
                return View("~/Views/Feature/Dashboard/Dashboard/Partial/_billCompare.cshtml", billcompareModel);
            }
            return new EmptyResult();
        }
    }
}