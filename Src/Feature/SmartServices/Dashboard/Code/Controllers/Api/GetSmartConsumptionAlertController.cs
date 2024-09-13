// <copyright file="GetSmartConsumptionAlertController.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Dashboard.Controllers.Api
{
    using DEWAXP.Feature.Dashboard.Models.SmartAlerts;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers.Api;
    using DEWAXP.Foundation.Content.Filters.Http;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.APIHandler.Clients;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Premise;
    using DEWAXP.Foundation.Integration.DewaSvc;
    using DEWAXP.Foundation.Integration.Responses;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="GetSmartConsumptionAlertController" />.
    /// </summary>
    [TwoPhaseAuthorize]
    public class GetSmartConsumptionAlertController : BaseApiController
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
                    bool eMetering;
                    bool wMetering;
                    IsSmartMetering(id, out eMetering, out wMetering);
                    if (eMetering || wMetering)
                    {
                        ServiceResponse<smartAlertReadingOut> consumptionResponse = DewaApiClient.GetSmartConsumptionAlert(
                            new GetSmartConsumptionAlert
                            {
                                GetSmartConsumptionAlert1 = new smartAlertReadingIn
                                {
                                    contractAccount = id,
                                    credential = CurrentPrincipal.SessionToken
                                }
                            }, RequestLanguage, Request.Segment());

                        if (consumptionResponse.Succeeded)
                        {
                            if (consumptionResponse.Payload != null)
                            {
                                ServiceResponse<AccountDetails[]> response;
                                if (!CacheProvider.TryGet(CacheKeys.ACCOUNT_LIST, out response) || response.Payload.Length < 1)
                                {
                                    //response = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());
                                    response = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());

                                    if (response.Succeeded)
                                    {
                                        CacheProvider.Store(CacheKeys.ACCOUNT_LIST, new CacheItem<ServiceResponse<AccountDetails[]>>(response, TimeSpan.FromHours(1)));
                                    }
                                }
                                GetSmartConsumptionResponse responsedata = GetSmartConsumptionResponse.From(consumptionResponse.Payload, id, response);
                                return Request.CreateResponse(HttpStatusCode.OK, new { response = responsedata });
                            }
                            ErrorMessage = "No consumption data available";
                        }
                        ErrorMessage = consumptionResponse.Message;
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { response = "", emetering = eMetering, wmetering = wMetering });
                    }
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

        private bool IsSmartMetering(string contractAccNo, out bool eSmartMetering, out bool wSmartMetering)
        {
            eSmartMetering = false;
            wSmartMetering = false;

            try
            {
                var _issueRepsonse = PremiseClient.GetDetails(new PremiseDetailsRequest()
                {
                    PremiseDetailsIN = new PremiseDetailsIN()
                    {
                        contractaccount = contractAccNo,
                        dminfo = false,
                        meterstatusinfo = true,
                        outageinfo = false,
                        podcustomer = false,
                        seniorcitizen = false,
                        userid = CurrentPrincipal.Username,
                        sessionid = CurrentPrincipal.SessionToken
                    }
                }, RequestLanguage, Request.Segment());


                if (_issueRepsonse.Succeeded && _issueRepsonse.Payload != null)
                {
                    var _responseData = _issueRepsonse.Payload;

                    eSmartMetering = _responseData?.meter?.electricitySmartMeter == true ? true : false;
                    wSmartMetering = _responseData?.meter?.waterSmartMeter == true ? true : false;

                    return true;
                }
            }
            catch (System.Exception ex)
            {
                Foundation.Logger.LogService.Error(ex, this);
            }

            return false;
        }
    }
}