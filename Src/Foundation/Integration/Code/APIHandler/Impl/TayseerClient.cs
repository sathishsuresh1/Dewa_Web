using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Requests.Tayseer;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.Tayseer;
using DEWAXP.Foundation.Logger;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(ITayseerClient), Lifetime = Lifetime.Transient)]
    public class TayseerClient : BaseApiDewaGateway, ITayseerClient
    {
        private const string errorMessage = "response value: Status : {0} Content: {1} Description: {2}";

        public ServiceResponse<TayseerCreateResponse> TayseerCreateReference(TayseerDetailsRequest request, string userId, string userSessionId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.referencenumberinputs.lang = language.Code();
                request.referencenumberinputs.appversion = AppVersion;
                request.referencenumberinputs.mobileosversion = AppVersion;
                request.referencenumberinputs.sessionid = userSessionId;
                request.referencenumberinputs.userid = userId;
                request.referencenumberinputs.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute($"{ApiBaseConfig.SmartCustomerV3_ApiUrl}" + "tayseer/", "create", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TayseerCreateResponse _response = CustomJsonConvertor.DeserializeObject<TayseerCreateResponse>(response.Content);

                    if (_response != null &&
                        !string.IsNullOrWhiteSpace(Convert.ToString(_response?.responsecode)) &&
                        _response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<TayseerCreateResponse>(_response, true, _response.description);
                    }
                    else
                    {
                        LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                        return new ServiceResponse<TayseerCreateResponse>(null, false, _response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                }

            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return new ServiceResponse<TayseerCreateResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

        public ServiceResponse<TayseerDetailsResponse> TayseerDetails(TayseerDetailsRequest request, string userId, string userSessionId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.referencenumberinputs.lang = language.Code();
                request.referencenumberinputs.appversion = AppVersion;
                request.referencenumberinputs.mobileosversion = AppVersion;
                request.referencenumberinputs.sessionid = userSessionId;
                request.referencenumberinputs.userid = userId;
                request.referencenumberinputs.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute($"{ApiBaseConfig.SmartCustomerV3_ApiUrl}" + "tayseer/", "details", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TayseerDetailsResponse _response = CustomJsonConvertor.DeserializeObject<TayseerDetailsResponse>(response.Content);

                    if (_response != null &&
                        !string.IsNullOrWhiteSpace(Convert.ToString(_response?.responsecode)) &&
                        _response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<TayseerDetailsResponse>(_response, true, _response.description);
                    }
                    else
                    {
                        LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                        return new ServiceResponse<TayseerDetailsResponse>(null, false, _response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                }

            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return new ServiceResponse<TayseerDetailsResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

        public ServiceResponse<TayseerListResponse> TayseerList(TayseerDetailsRequest request, string userId, string userSessionId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.referencenumberinputs = new Referencenumberinputs
                {
                    lang = language.Code(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    sessionid = userSessionId,
                    userid = userId,
                    vendorid = GetVendorId(segment)
                };
                IRestResponse response = DewaApiExecute($"{ApiBaseConfig.SmartCustomerV3_ApiUrl}" + "tayseer/", "list", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TayseerListResponse _response = CustomJsonConvertor.DeserializeObject<TayseerListResponse>(response.Content);

                    if (_response != null &&
                        !string.IsNullOrWhiteSpace(Convert.ToString(_response?.responsecode)) &&
                        _response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<TayseerListResponse>(_response, true, _response.description);
                    }
                    else
                    {
                        LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                        return new ServiceResponse<TayseerListResponse>(null, false, _response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                }

            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return new ServiceResponse<TayseerListResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }
    }
}
