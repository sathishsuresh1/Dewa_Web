// <copyright file="EVDashboardController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Controllers
{
    using DEWAXP.Foundation.Content.Filters.Mvc;
    using DEWAXP.Feature.EV.Models.EVDashboard;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content.Models.Bills;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
    using global::Sitecore.Mvc.Presentation;
    using System.Web.Mvc;

    /// <summary>
    /// Defines the <see cref="EVDashboardController" />.
    /// </summary>
    [TwoPhaseAuthorize(AllowEVUsers = true)]
    public class EVDashboardController : BaseController
    {
        [HttpGet]
        public ActionResult EVDashboardHeader()
        {
            return View("~/Views/Feature/EV/EVDashboard/EVDashboardHeader.cshtml");
        }

        /// <summary>
        /// The ListofEVCards.
        /// </summary>
        /// <returns>The <see cref="PartialViewResult"/>.</returns>
        [HttpGet]
        public ActionResult ListofEVCards()
        {
            return View("~/Views/Feature/EV/EVDashboard/ListofEVCards.cshtml");
        }

        [HttpGet]
        public ActionResult EVConsumption()
        {
            return View("~/Views/Feature/EV/EVDashboard/EVConsumption.cshtml");
        }

        [HttpGet]
        public ActionResult EVHome()
        {
            return View("~/Views/Feature/EV/EVDashboard/EVHome.cshtml");
        }

        [HttpGet]
        public ActionResult EVTransactions()
        {
            if (RenderingRepository.HasDataSource)
            {
                EVTransactionsConfig Configdatasource = RenderingRepository.GetDataSourceItem<EVTransactionsConfig>();
                if (Configdatasource != null)
                {
                    EVTransactionModel eVTransactionModel = new EVTransactionModel { eVTransactionsConfig = Configdatasource };
                    EVTransactionCache eVTransactionCache;
                    if (CacheProvider.TryGet(CacheKeys.EV_Transaction_List, out eVTransactionCache))
                    {
                        if (eVTransactionCache != null)
                        {
                            eVTransactionModel.Selectedaccountnumber = eVTransactionCache.accountnumber;
                            eVTransactionModel.Selectedcardid = eVTransactionCache.cardid;
                            eVTransactionModel.Selectedmonth = eVTransactionCache.month;
                            eVTransactionModel.Selectedsortby = eVTransactionCache.sortby;
                        }
                    }

                    return View("~/Views/Feature/EV/EVDashboard/EVTransactions.cshtml", eVTransactionModel);
                }
            }
            return new EmptyResult();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EVBillCompare(string account)
        {
            if (!string.IsNullOrWhiteSpace(account))
            {
                DEWAXP.Foundation.Integration.Responses.ServiceResponse<BillHistoryResponse> Response = SmartCustomerClient.GetBillPaymentHistory(
                        new BillHistoryRequest
                        {
                            contractaccountnumber = account,
                            sessionid = CurrentPrincipal.SessionToken,
                            userid = CurrentPrincipal.UserId
                        }, RequestLanguage, Request.Segment());
                CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(account.TrimStart(new char[] { '0' }), Times.Once));
                BillCompareViewModel billcompareModel = BillCompareViewModel.FromAPI(Response.Payload, true);
                return View("~/Views/Feature/EV/EVDashboard/Partial/_billCompare.cshtml", billcompareModel);
            }
            return View("~/Views/Feature/EV/EVDashboard/Partial/_billCompare.cshtml", new BillCompareViewModel());
        }
    }
}