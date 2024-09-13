// <copyright file="EVBillsController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Controllers.Api
{
    using DEWAXP.Feature.EV.Models.EVDashboard;
    using DEWAXP.Foundation.Content.Controllers.Api;
    using DEWAXP.Foundation.Content.Filters.Http;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
    using DEWAXP.Foundation.Logger;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Defines the <see cref="EVBillsController" />.
    /// </summary>
    [TwoPhaseAuthorize]
    public class EVBillsController : EVDashboardBaseController
    {
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="accountnumber">The accountnumber<see cref="string"/>.</param>
        /// <param name="cardid">The cardid<see cref="string"/>.</param>
        /// <param name="month">The month<see cref="string"/>.</param>
        /// <param name="year">The year<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{HttpResponseMessage}"/>.</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Totalamount(string accountnumber)
        {
            string ErrorMessage = "";
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    FutureCenterValues _fc = FetchFutureCenterValues();
                    DEWAXP.Foundation.Integration.Responses.ServiceResponse<List<Outstandingbreakdown>> Response = EVDashboardClient.GetOutstandingBreakDown(
                           new EVBillsRequest
                           {
                               outstandingbreakdownIN = new OutstandingbreakdownIN
                               {
                                   contractaccount = accountnumber,
                                   sessionid = CurrentPrincipal.SessionToken,
                                   center = _fc.Branch,
                               }
                           }, RequestLanguage, Request.Segment());
                    if (Response.Succeeded && Response.Payload != null && Response.Payload.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { data = EVBillsViewModel.From(Response.Payload.FirstOrDefault()) });
                    }
                    ErrorMessage = Response.Message;
                    LogService.Info(new Exception(ErrorMessage));
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage);
            }))());
        }
    }
}