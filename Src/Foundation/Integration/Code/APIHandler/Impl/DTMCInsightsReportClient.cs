using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.DTMCInsightsReport;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.meterreading;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.DTMCInsightsReport;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.meterreading;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IDTMCInsightsReportClient), Lifetime = Lifetime.Transient)]
    public class DTMCInsightsReportClient : BaseApiDewaGateway, Clients.IDTMCInsightsReportClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";
        private string BaseSmartMeterV2ApiUrl => $"{ApiBaseConfig.SmartMeterV2_ApiUrl}";

        public ServiceResponse<GetWaterAIResponse> GetWaterAI(GetWaterAIRequest request, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                //request.dsn = "17Y5011527";
                //request.from = "2020-10-19";
                //request.to = "2020-10-26";
                //request.sessionid = "DWEB82CC96B85269FDC431";

                IRestResponse response = DewaApiExecute(BaseApiUrl, "waterai", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    GetWaterAIResponse _response = CustomJsonConvertor.DeserializeObject<GetWaterAIResponse>(response.Content);
                    if (_response != null && !string.IsNullOrWhiteSpace(_response.ResponseCode) && _response.ResponseCode.Equals("000"))
                    {
                        return new ServiceResponse<GetWaterAIResponse>(_response);
                    }
                    else
                    {
                        return new ServiceResponse<GetWaterAIResponse>(null, false, _response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<GetWaterAIResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<GetWaterAIResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<GetWaterAIConsumptionResponse> GetWaterAIConsumption(GetWaterAIConsumptionRequest request, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {

                IRestResponse response = DewaApiExecute(BaseSmartMeterV2ApiUrl, "meterreading/aiwater", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    GetWaterAIConsumptionResponse _response = CustomJsonConvertor.DeserializeObject<GetWaterAIConsumptionResponse>(response.Content);
                    if (_response != null && !string.IsNullOrWhiteSpace(_response.ReplyMessage?.Reply?.replyCode) && (_response.ReplyMessage.Reply.replyCode.Equals("000") || _response.ReplyMessage.Reply.replyCode.Equals("0")))
                    {
                        return new ServiceResponse<GetWaterAIConsumptionResponse>(_response);
                    }
                    else
                    {
                        return new ServiceResponse<GetWaterAIConsumptionResponse>(null, false, _response.ReplyMessage.Reply.replyText);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<GetWaterAIConsumptionResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<GetWaterAIConsumptionResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        /// <summary>
        /// Mayank parekh For Unbilled Consumption
        /// </summary>
        /// <param name="request"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        public ServiceResponse<MeterreadingResponse> GetMeterReading(MeterreadingRequest request, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseSmartMeterV2ApiUrl, "meterreading/unbilled", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MeterreadingResponse _response = CustomJsonConvertor.DeserializeObject<MeterreadingResponse>(response.Content);
                    if (_response != null && !string.IsNullOrWhiteSpace(_response.errorCodeE) && !string.IsNullOrWhiteSpace(_response.errorCodeW) && (_response.errorCodeE.Equals("000")|| _response.errorCodeW.Equals("000")))
                    {
                        return new ServiceResponse<MeterreadingResponse>(_response);
                    }
                    else
                    {
                        return new ServiceResponse<MeterreadingResponse>(null, false,Convert.ToString(_response.errorMessageE+"|"+ _response.errorMessageW));
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<MeterreadingResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<MeterreadingResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
