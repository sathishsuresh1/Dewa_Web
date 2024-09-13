using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.DTMCTracking;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.DTMCTracking;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IDTMCTrackingClient), Lifetime = Lifetime.Transient)]
    public class DTMCTrackingClient : BaseApiDewaGateway, Clients.IDTMCTrackingClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";
        public ServiceResponse<LocationStatusResponse> GetLocationStatus(LocationStatusRequest request, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.notificationdtmcinput.appversion = AppVersion;
                request.notificationdtmcinput.appidentifier = segment.Identifier();
                request.notificationdtmcinput.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, "locationstatus", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    LocationStatusResponse _Response = CustomJsonConvertor.DeserializeObject<LocationStatusResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<LocationStatusResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<LocationStatusResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<LocationStatusResponse>(null, false, $"response value: '{response}'");
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<LocationStatusResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<NotificationStatusResponse> GetNotificationStatus(NotificationStatusRequest request, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.notificationdtmcinput.appversion = AppVersion;
                request.notificationdtmcinput.appidentifier = segment.Identifier();
                request.notificationdtmcinput.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, "notificationstatus", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    NotificationStatusResponse _Response = CustomJsonConvertor.DeserializeObject<NotificationStatusResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<NotificationStatusResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<NotificationStatusResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<NotificationStatusResponse>(null, false, $"response value: '{response}'");
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<NotificationStatusResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
