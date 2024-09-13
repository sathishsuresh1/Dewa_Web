using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.AnonymousTrack;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IAnonymousTrackingClient), Lifetime = Lifetime.Transient)]
    public class AnonymousTrackingClient : BaseApiDewaGateway, IAnonymousTrackingClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";

        #region Universal Service Centre - General Track Request
        public ServiceResponse<GeneralTrackResponse> GetGeneralTrackDetail(GeneralTrackRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.trackrequestparams.lang = language.Code();
                request.trackrequestparams.vendorid = GetVendorId(segment);
                request.trackrequestparams.appversion = AppVersion;
                request.trackrequestparams.appidentifier = segment.Identifier();
                request.trackrequestparams.mobileosversion = AppVersion;

                request.trackrequestparams.applicationflag = "M";
                request.trackrequestparams.logflag = "A";

                request.trackrequestparams.vendorid = "EPAY";//GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, "generaltrackrequest", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    GeneralTrackResponse _Response = CustomJsonConvertor.DeserializeObject<GeneralTrackResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<GeneralTrackResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<GeneralTrackResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<GeneralTrackResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<GeneralTrackResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        #endregion

        #region Universal Service Centre - Track Request List
        public ServiceResponse<TrackListResponse> GetNotificationList(TrackListRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.complaintsIn.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "trackrequestlist", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TrackListResponse _Response = CustomJsonConvertor.DeserializeObject<TrackListResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<TrackListResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<TrackListResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<TrackListResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<TrackListResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        #endregion

        #region Verify OTP
        public ServiceResponse<TrackVerifyOtpResponse> VerifyOtp(TrackVerifyOtpRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "verifyotp", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TrackVerifyOtpResponse _Response = CustomJsonConvertor.DeserializeObject<TrackVerifyOtpResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responseCode) && _Response.responseCode.Equals("000"))
                    {
                        return new ServiceResponse<TrackVerifyOtpResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<TrackVerifyOtpResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<TrackVerifyOtpResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<TrackVerifyOtpResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        #endregion
    }
}
