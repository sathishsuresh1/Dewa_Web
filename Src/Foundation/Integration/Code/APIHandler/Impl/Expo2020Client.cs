using System;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Expo2020;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Expo2020;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;

using RestSharp;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IExpo2020Client), Lifetime = Lifetime.Transient)]
    public class Expo2020Client : BaseApiDewaGateway, IExpo2020Client
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";

        public ServiceResponse<Expo2020Response> FeedbackExpo(Expo2020Request request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.smartformsubmission.lang = language.Code();
                request.smartformsubmission.vendorid = GetVendorId(segment);
                request.smartformsubmission.appversion = AppVersion;
                request.smartformsubmission.appidentifier = segment.Identifier();
                request.smartformsubmission.mobileosversion = AppVersion;

                IRestResponse response = DewaApiExecute(BaseApiUrl, "feedbackexpo", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Expo2020Response _Response = CustomJsonConvertor.DeserializeObject<Expo2020Response>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responseCode) && _Response.responseCode.Equals("000"))
                    {
                        return new ServiceResponse<Expo2020Response>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<Expo2020Response>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<Expo2020Response>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<Expo2020Response>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        public ServiceResponse<Expo2020MasterDataResponse> MasterDataExpo(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                var request = new Expo2020MasterDataRequest
                {
                    lang = language.Code(),
                    vendorid = GetVendorId(segment),
                    appversion = AppVersion,
                    appidentifier = segment.Identifier(),
                    mobileosversion = AppVersion
                };

                IRestResponse response = DewaApiExecute(BaseApiUrl, "masterdataexpo", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Expo2020MasterDataResponse _Response = CustomJsonConvertor.DeserializeObject<Expo2020MasterDataResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responseCode) && _Response.responseCode.Equals("000"))
                    {
                        return new ServiceResponse<Expo2020MasterDataResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<Expo2020MasterDataResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<Expo2020MasterDataResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<Expo2020MasterDataResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
