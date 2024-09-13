using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.AwayMode;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;

using RestSharp;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IAwayModeClient), Lifetime = Lifetime.Transient)]
    public class AwayModeClient : BaseApiDewaGateway, Clients.IAwayModeClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";

        public ServiceResponse<ManageAwayModeResponse> Manageawaymode(ManageAwayModeRequest request, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.vendor = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, "manageawaymode", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ManageAwayModeResponse _Response = CustomJsonConvertor.DeserializeObject<ManageAwayModeResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responseCode) && _Response.responseCode.Equals("000"))
                    {
                        return new ServiceResponse<ManageAwayModeResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<ManageAwayModeResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<ManageAwayModeResponse>(null, false, $"response value: '{response}'");
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ManageAwayModeResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        public ServiceResponse<ConsumptionDataResponse> GetConsumptionData(string code, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                
                IRestResponse response = DewaApiExecute(BaseApiUrl, "getawaymodegraph", new { code = code }, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ConsumptionDataResponse _Response = CustomJsonConvertor.DeserializeObject<ConsumptionDataResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responseCode) && _Response.responseCode.Equals("000"))
                    {
                        return new ServiceResponse<ConsumptionDataResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<ConsumptionDataResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<ConsumptionDataResponse>(null, false, $"response value: '{response}'");
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ConsumptionDataResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        //#region [Async]

        //public async Task<ServiceResponse<ManageAwayModeResponse>> ManageawaymodeAsync(ManageAwayModeRequest request, RequestSegment segment = RequestSegment.Desktop)
        //{
        //    try
        //    {

        //        request.vendor = GetVendorId(segment);
        //        IRestResponse response = await DewaApiExecuteAsync(BaseApiUrl, "manageawaymode", request, Method.POST, null);
        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            ManageAwayModeResponse _Response = CustomJsonConvertor.DeserializeObject<ManageAwayModeResponse>(response.Content);
        //            if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responseCode) && _Response.responseCode.Equals("000"))
        //            {
        //                return new ServiceResponse<ManageAwayModeResponse>(_Response);
        //            }
        //            else
        //            {
        //                return new ServiceResponse<ManageAwayModeResponse>(null, false, _Response.description);
        //            }
        //        }
        //        else
        //        {
        //            LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
        //            return new ServiceResponse<ManageAwayModeResponse>(null, false, $"response value: '{response}'");
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogService.Fatal(ex, this);
        //    }
        //    return new ServiceResponse<ManageAwayModeResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        //}

        //#endregion
    }
}
