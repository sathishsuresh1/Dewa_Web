using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.UAEPassService;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.UAEPassService;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web.Configuration;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IUAEPassServiceClient), Lifetime = Lifetime.Transient)]
    public class UAEPassServiceClient : BaseApiDewaGateway, IUAEPassServiceClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.RestAPIUrl_SmartDubai}";

        public ServiceResponse<UAEPassDubaiIdLoginResponse> UAEPASSDubaiIdLogin(UAEPassDubaiIdLoginRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                #region [Api Request ]

                var uaeloginRequest = new { appversion = AppVersion, mobileosversion = AppVersion, lang = language.Code(), loginmode = "2", appidentifier = segment.Identifier() };
                Dictionary<string, string> requestHeader = new Dictionary<string, string>();
                requestHeader.Add("code", request.code);
                requestHeader.Add("merchantid", WebConfigurationManager.AppSettings["Jobseeker_New_UserName"]);
                requestHeader.Add("password", WebConfigurationManager.AppSettings["Jobseeker_New_Password"]);
                requestHeader.Add("vendorid", GetJobSeekerVendorId(segment));

                #endregion [Api Request ]

                IRestResponse response = DewaApiExecute(BaseApiUrl, "v2/login/uaeiid/dubaiidlogin", uaeloginRequest, Method.POST, null, requestHeader);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UAEPassDubaiIdLoginResponse _Response = CustomJsonConvertor.DeserializeObject<UAEPassDubaiIdLoginResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<UAEPassDubaiIdLoginResponse>(_Response, true, _Response.description);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {_Response.responsecode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<UAEPassDubaiIdLoginResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<UAEPassDubaiIdLoginResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<UAEPassDubaiIdLoginResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<dynamic> UAEPASSCustomerAuthentication(UAEPassDubaiIdLoginRequest input)
        {
            try
            {
                var client = new RestClient(UAEPassConfig.UAEPASS_OAUTH_URL);
                client.Authenticator = new HttpBasicAuthenticator(UAEPassConfig.UAEPASS_CLIENT_ID, UAEPassConfig.UAEPASS_CLIENT_SECRETID); ;
                client.Proxy = new WebProxy(ConfigurationManager.AppSettings["PROXYURL"]);
                client.Proxy.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["PROXYUSER"], ConfigurationManager.AppSettings["PROXYPASSWORD"], ConfigurationManager.AppSettings["PROXYDOMAIN"]);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("redirect_uri", UAEPassConfig.UAEPASS_RETURNOAUTHURL);
                request.AddParameter("code", input.code);
                IRestResponse response = client.Execute(request);
                if (response != null && response.StatusCode.Equals(HttpStatusCode.OK) && !string.IsNullOrWhiteSpace(response.Content))
                {
                    dynamic dynamicobj = JObject.Parse(response.Content);
                    if (dynamicobj != null)
                    {
                        if (dynamicobj.error != null && dynamicobj.error_description != null)
                        {
                            return new ServiceResponse<dynamic>(null, false, (string)dynamicobj.error_description);
                        }
                        if (dynamicobj.access_token != null && dynamicobj.token_type != null)
                        {
                            return new ServiceResponse<dynamic>(dynamicobj, true, string.Empty);
                        }
                    }
                }
                else if (response != null && !string.IsNullOrWhiteSpace(response.Content))
                {
                    dynamic dynamicobj = JObject.Parse(response.Content);
                    if (dynamicobj != null)
                    {
                        if (dynamicobj.error != null && dynamicobj.error_description != null)
                        {
                            return new ServiceResponse<dynamic>(null, false, (string)dynamicobj.error_description);
                        }
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<dynamic>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<dynamic>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<string> UAEPASSCustomerData(string access_token, string token_type)
        {
            try
            {
                var clientinfo = new RestClient(UAEPassConfig.UAEPASS_USER_INFO);
                clientinfo.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(access_token, token_type);
                clientinfo.Proxy = new WebProxy(ConfigurationManager.AppSettings["PROXYURL"]);
                clientinfo.Proxy.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["PROXYUSER"], ConfigurationManager.AppSettings["PROXYPASSWORD"], ConfigurationManager.AppSettings["PROXYDOMAIN"]);
                var requestInfo = new RestRequest(Method.GET);
                IRestResponse response = clientinfo.Execute(requestInfo);
                if (response != null && response.StatusCode.Equals(HttpStatusCode.OK) && !string.IsNullOrWhiteSpace(response.Content))
                {
                    return new ServiceResponse<string>(response.Content, true, string.Empty);
                }
                else if (response != null && !string.IsNullOrWhiteSpace(response.Content))
                {
                    dynamic dynamicobj = JObject.Parse(response.Content);
                    if (dynamicobj != null)
                    {
                        if (dynamicobj.error != null && dynamicobj.error_description != null)
                        {
                            return new ServiceResponse<string>(string.Empty, false, (string)dynamicobj.error_description);
                        }
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<string>(string.Empty, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<string>(string.Empty, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}