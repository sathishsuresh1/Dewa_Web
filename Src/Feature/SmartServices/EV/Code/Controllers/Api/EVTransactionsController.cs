// <copyright file="EVTransactionsController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Controllers.Api
{
    using DEWAXP.Feature.EV.Models.EVDashboard;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Filters.Http;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
    using DEWAXP.Foundation.Logger;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Defines the <see cref="EVTransactionsController" />.
    /// </summary>
    [TwoPhaseAuthorize]
    public class EVTransactionsController : EVDashboardBaseController
    {
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="accountnumber">The accountnumber<see cref="string"/>.</param>
        /// <param name="cardid">The cardid<see cref="string"/>.</param>
        /// <param name="monthyear">The monthyear<see cref="string"/>.</param>
        /// <param name="sortby">The sortby<see cref="string"/>.</param>
        /// <param name="dashboardpage">The dashboardpage<see cref="bool"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{HttpResponseMessage}"/>.</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Account(string accountnumber, string cardid, string monthyear, string sortby = "", bool dashboardpage = true, int page = 1)
        {
            string ErrorMessage = "";
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                if (!string.IsNullOrWhiteSpace(accountnumber))
                {
                    try
                    {
                        DEWAXP.Foundation.Integration.Responses.ServiceResponse<EVTransactionalResponse> Response = EVDashboardClient.GetEVTransactionalDetails(
                        new EVConsumptionRequest
                        {
                            contractaccount = accountnumber,
                            sessionid = CurrentPrincipal.SessionToken,
                            cardnumber = cardid,
                            month = !string.IsNullOrWhiteSpace(monthyear) ? long.Parse(monthyear).ToString("D6").Substring(0, 2) : string.Empty,
                            year = !string.IsNullOrWhiteSpace(monthyear) ? long.Parse(monthyear).ToString("D6").Substring(2, 4) : string.Empty,
                        }, RequestLanguage, Request.Segment());
                        CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(accountnumber.TrimStart(new char[] { '0' }), Times.Once));
                        CacheProvider.Store(CacheKeys.EV_Transaction_List, new AccessCountingCacheItem<EVTransactionCache>(new EVTransactionCache
                        {
                            accountnumber = accountnumber.TrimStart(new char[] { '0' }),
                            cardid = cardid,
                            month = monthyear,
                            sortby = sortby
                        }, Times.Once));
                        if (Response.Succeeded)
                        {
                            if (Response.Payload != null)
                            {
                                EVTransactionListViewModel transactionresponse = EVTransactionListViewModel.From(Response.Payload, dashboardpage, page, sortby);
                                return Request.CreateResponse(HttpStatusCode.OK, new { data = transactionresponse });
                            }
                            ErrorMessage = "No available cards";
                        }
                        ErrorMessage = Response.Message;
                        LogService.Info(new System.Exception(ErrorMessage));
                        return Request.CreateResponse(HttpStatusCode.OK, new { Error = ErrorMessage });
                    }
                    catch (System.Exception ex)
                    {
                        ErrorMessage = ex.Message;
                    }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage);
            }))());
        }
    }
}