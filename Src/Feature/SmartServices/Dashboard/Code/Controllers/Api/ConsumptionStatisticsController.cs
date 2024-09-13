using DEWAXP.Feature.Dashboard.Models.ConsumptionStatistics;
using DEWAXP.Feature.Dashboard.Models.Graph;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Content.Models.ConsumptionStatistics;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Premise;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Helpers;
using System.Web.Http;

namespace DEWAXP.Feature.Dashboard.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class ConsumptionStatisticsController : BaseApiController
    {
        [HttpPost]
        public HttpResponseMessage Consumption([FromBody] AMIConsumptionAccount account)
        {
            try
            {
                AntiForgery.Validate();
                string id = account != null ? account.id : string.Empty;
                string Errormessage = "No consumption data available";
                if (!string.IsNullOrWhiteSpace(id))
                {
                    string consumptionaccount;
                    ServiceResponse<YearlyConsumptionDataResponse> consumptionResponse = null;
                    if (CacheProvider.TryGet(CacheKeys.SELECTED_CONSUMPTIONACCOUNT, out consumptionaccount))
                    {
                        if (!string.IsNullOrEmpty(consumptionaccount) && consumptionaccount.Equals(id))
                        {
                            if (CacheProvider.TryGet(CacheKeys.SELECTED_CONSUMPTION, out consumptionResponse))
                            {
                            }
                        }
                    }
                    if (consumptionResponse == null)
                    {
                        consumptionResponse = DewaApiClient.GetConsumptionHistory(CurrentPrincipal.SessionToken, id, RequestLanguage, Request.Segment());
                        CacheProvider.Store(CacheKeys.SELECTED_CONSUMPTION, new CacheItem<ServiceResponse<YearlyConsumptionDataResponse>>(consumptionResponse, TimeSpan.FromMinutes(20)));
                        CacheProvider.Store(CacheKeys.SELECTED_CONSUMPTIONACCOUNT, new CacheItem<string>(id, TimeSpan.FromMinutes(20)));
                    }
                    if (consumptionResponse != null && consumptionResponse.Succeeded)
                    {
                        if (consumptionResponse.Payload != null)
                        {
                            var @return = DataSeries.Create(consumptionResponse.Payload);
                            var model = new GraphRenderingModel();
                            IsSmartMetering(id, model);

                            return Request.CreateResponse(HttpStatusCode.OK, new { series = @return, meter = model });
                        }
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, Errormessage);
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, consumptionResponse.Message);
                }
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, Errormessage);
            }
            catch (System.Web.Mvc.HttpAntiForgeryException ex)
            {
                LogService.Fatal(ex, this);
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private bool IsSmartMetering(string contractAccNo, GraphRenderingModel model)
        {
            model.IsSmartElectricityMeter = false;
            model.IsSmartWaterMeter = false;
            //eSmartMetering = false;
            //wSmartMetering = false;

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
                    model.IsSmartElectricityMeter = _responseData?.meter?.electricitySmartMeter == true ? (_responseData?.meter?.electricitymeterType?.Equals("03") == false ? true : false) : false;
                    model.IsSmartWaterMeter = _responseData?.meter?.waterSmartMeter == true ? (_responseData?.meter?.watermeterType?.Equals("03") == false ? true : false) : false;
                    model.MoveInDate = _responseData?.meter?.moveinDate;
                    model.MoveOutDate = _responseData?.meter?.moveoutDate;
                    //eSmartMetering = _responseData?.meter?.electricitySmartMeter == true ? true : false;
                    //wSmartMetering = _responseData?.meter?.waterSmartMeter == true ? true : false;

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return false;
        }
    }

    [DataContract]
    public class AMIConsumptionAccount
    {
        [DataMember]
        public string id { get; set; }
    }
}