using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartCommunication;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.SmartCommunication;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(ISmartCommunicationClient), Lifetime = Lifetime.Transient)]
    public class SmartCommunicationClient : BaseApiDewaGateway, ISmartCommunicationClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";

        public ServiceResponse<SmartCommunicationResponse> SmartCommunicationSubmit(SmartCommunicationRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {

                request.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "notification/anonymous", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SmartCommunicationResponse _Response = CustomJsonConvertor.DeserializeObject<SmartCommunicationResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<SmartCommunicationResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<SmartCommunicationResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<SmartCommunicationResponse>(null, false, $"response value: '{response}'");
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<SmartCommunicationResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

        public ServiceResponse<SmartCommunicationVerifyOtpResponse> VerifyMobileOtp(SmartCommunicationVerifyOtpRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {

            try
            {
                request.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "verifyotp", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SmartCommunicationVerifyOtpResponse _Response = CustomJsonConvertor.DeserializeObject<SmartCommunicationVerifyOtpResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responseCode) && _Response.responseCode.Equals("000"))
                    {
                        return new ServiceResponse<SmartCommunicationVerifyOtpResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<SmartCommunicationVerifyOtpResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<SmartCommunicationVerifyOtpResponse>(null, false, $"response value: '{response}'");
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<SmartCommunicationVerifyOtpResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }
        public ServiceResponse<SessionLoginResponse> SessionLogin(SessionLoginRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {

            try
            {
                request.sessionparams.lang = language.Code();
                request.sessionparams.merchantid = GetUSCMerchantId(segment); //"UNICRQUID";
                request.sessionparams.merchantpassword = GetUSCMerchantPassword(segment);//"JW762VCN0S156938";

                IRestResponse response = DewaApiExecute(BaseApiUrl, "sessionlogin", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SessionLoginResponse _Response = CustomJsonConvertor.DeserializeObject<SessionLoginResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<SessionLoginResponse>(_Response);
                    }
                    else
                    {
                        if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && !string.IsNullOrWhiteSpace(_Response.maxattempt) && _Response.maxattempt.Equals("X"))
                            return new ServiceResponse<SessionLoginResponse>(_Response, false, _Response.description);
                        else
                            return new ServiceResponse<SessionLoginResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<SessionLoginResponse>(null, false, $"response value: '{response}'");
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<SessionLoginResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

        public ServiceResponse<CustomerUpdateOtpResponse> CustomerVerifyOtp(SmartCommunicationVerifyOtpRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "verifyotp", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    CustomerUpdateOtpResponse _Response = CustomJsonConvertor.DeserializeObject<CustomerUpdateOtpResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<CustomerUpdateOtpResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<CustomerUpdateOtpResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<CustomerUpdateOtpResponse>(null, false, $"response value: '{response}'");
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<CustomerUpdateOtpResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }
    }
}
