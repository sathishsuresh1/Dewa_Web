// <copyright file="GetSmartConsumptionAlertController.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Dashboard.Controllers.Api
{
    using DEWAXP.Feature.Dashboard.Models.SmartAlerts;
    using DEWAXP.Foundation.Content.Controllers.Api;
    using DEWAXP.Foundation.Content.Filters.Http;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.DewaSvc;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="GetSlabCellingSmartAlertController" />.
    /// </summary>
    [TwoPhaseAuthorize]
    public class GetSlabCellingSmartAlertController : BaseApiController
    {
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{HttpResponseMessage}"/>.</returns>
        public async Task<HttpResponseMessage> Get(string id)
        {
            string ErrorMessage = "";
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    var SlabResponse = DewaApiClient.GetSmartAlert(
                        new GetSmartAlert
                        {
                            GetSmartAlert1 = new smartAlertSubrIn
                            {
                                contractAccount = id,
                                credential = CurrentPrincipal.SessionToken
                            }
                        }, RequestLanguage, Request.Segment());

                    if (SlabResponse.Succeeded)
                    {
                        if (SlabResponse.Payload != null)
                        {
                            var responsedata = GetSmartAlertResponseModel.From(SlabResponse.Payload.smartalert);
                            return Request.CreateResponse(HttpStatusCode.OK, new { response = responsedata });
                        }
                        ErrorMessage = "No consumption data available";
                    }
                    ErrorMessage = SlabResponse.Message;
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