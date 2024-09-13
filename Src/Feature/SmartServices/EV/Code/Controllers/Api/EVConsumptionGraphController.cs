// <copyright file="EVConsumptionGraphController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Controllers.Api
{
    using DEWAXP.Foundation.Content.Filters.Http;
    using DEWAXP.Foundation.Content.Models.ConsumptionStatistics;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
    using DEWAXP.Foundation.Logger;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Defines the <see cref="EVConsumptionGraphController" />.
    /// </summary>
    [TwoPhaseAuthorize]
    public class EVConsumptionGraphController : EVDashboardBaseController
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
        public async Task<HttpResponseMessage> Card(string accountnumber, string cardid, string month, string year)
        {
            string ErrorMessage = "";
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    DEWAXP.Foundation.Integration.Responses.ServiceResponse<EVConsumptionResponse> Response = EVDashboardClient.GetEVConsumptionDetails(
                           new EVConsumptionRequest
                           {
                               contractaccount = accountnumber,
                               sessionid = CurrentPrincipal.SessionToken,
                               cardnumber = cardid,
                               month = !string.IsNullOrWhiteSpace(month) ? long.Parse(month).ToString("D2") : string.Empty,
                               year = year
                           }, RequestLanguage, Request.Segment());
                    if (Response.Succeeded)
                    {
                        if (Response.Payload != null)
                        {
                            IList<DataSeries> @return = DataSeries.Create(Response.Payload);
                            return Request.CreateResponse(HttpStatusCode.OK, new { series = @return });
                        }
                        ErrorMessage = "No available cards";
                    }
                    ErrorMessage = Response.Message;
                    LogService.Info(new System.Exception(ErrorMessage));
                    IList<DataSeries> dataSeries = null;
                    return Request.CreateResponse(HttpStatusCode.OK, new { series = dataSeries });
                }
                catch (System.Exception ex)
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