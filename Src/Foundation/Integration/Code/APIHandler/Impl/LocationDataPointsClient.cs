using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.MoveOut;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.MoveOut;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using System;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.LocationDataPoints;
using static DEWAXP.Foundation.Integration.APIHandler.Models.Request.LocationDataPoints.DataPointsRequest;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(ILocationDataPointsClient), Lifetime = Lifetime.Transient)]
    public class LocationDataPointsClient : BaseApiDewaGateway, ILocationDataPointsClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";
        public ServiceResponse<DataPointsResponse> GetMapDataPoints(Root request)
        {
            try
            {
                request.onlinemapinputs.vendorid = GetVendorId(RequestSegment.Desktop);
                IRestResponse response = DewaApiExecute(BaseApiUrl, "ev/onlinemapdata", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    DataPointsResponse _Response = CustomJsonConvertor.DeserializeObject<DataPointsResponse>(response.Content);
                    if (_Response != null && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<DataPointsResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<DataPointsResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<DataPointsResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<DataPointsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
