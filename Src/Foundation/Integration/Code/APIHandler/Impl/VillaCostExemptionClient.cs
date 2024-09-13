using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Requests.VillaCostExemption;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.VillaCostExemption;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IVillaCostExemptionClient), Lifetime = Lifetime.Transient)]
    public class VillaCostExemptionClient : BaseApiDewaGateway, IVillaCostExemptionClient
    {
        private const string errorMessage = "response value: Status : {0} Content: {1} Description: {2}";
        public ServiceResponse<DashboardResponse> GetDashboardResources(string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                BaseRequest request = new BaseRequest()
                {
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code(),
                    mobileosversion = AppVersion,
                    sessionid = userSessionId,
                    userid = userId,
                    vendorid = GetVendorId(segment)
                };

                //var apiRequest = new { NewConnectionRefund = request };
#if DEBUG
                /*System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;*/
#endif
                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "hhdashboard", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    DashboardResponse model = CustomJsonConvertor.DeserializeObject<DashboardResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.Responsecode) && model.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<DashboardResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                        return new ServiceResponse<DashboardResponse>(null, false, model.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                    return new ServiceResponse<DashboardResponse>(null, false, response.StatusDescription);
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<DashboardResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<ApplicationResponse> GetApplicationResources(NewApplicationRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.villarequest.processcode = "01";
                request.villarequest.appidentifier = segment.Identifier();
                request.villarequest.appversion = AppVersion;
                request.villarequest.lang = language.Code();
                request.villarequest.mobileosversion = AppVersion;
                request.villarequest.sessionid = userSessionId;
                request.villarequest.userid = userId;
                request.villarequest.vendorid = GetVendorId(segment);
                request.villarequest.ownerdetails = new List<ownerdetails>();

#if DEBUG
                /*System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;*/
#endif
                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "hhvillamodify", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ApplicationResponse _response = CustomJsonConvertor.DeserializeObject<ApplicationResponse>(response.Content);
                    if (_response != null && !string.IsNullOrWhiteSpace(_response.Responsecode) && _response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<ApplicationResponse>(_response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                        return new ServiceResponse<ApplicationResponse>(null, false, _response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                    return new ServiceResponse<ApplicationResponse>(null, false, response.StatusDescription);
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ApplicationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<ApplicationResponse> SaveApplication(NewApplicationRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {

                request.villarequest.appidentifier = segment.Identifier();
                request.villarequest.appversion = AppVersion;
                request.villarequest.lang = language.Code();
                request.villarequest.mobileosversion = AppVersion;
                request.villarequest.sessionid = userSessionId;
                request.villarequest.userid = userId;
                request.villarequest.vendorid = GetVendorId(segment);

                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "hhvillamodify", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ApplicationResponse model = CustomJsonConvertor.DeserializeObject<ApplicationResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.Responsecode) && model.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<ApplicationResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                        return new ServiceResponse<ApplicationResponse>(null, false, model.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                    return new ServiceResponse<ApplicationResponse>(null, false, response.StatusDescription);
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ApplicationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<ApplicationResponse> RetrieveApplication(NewApplicationRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.villarequest.processcode = "01";

                request.villarequest.appidentifier = segment.Identifier();
                request.villarequest.appversion = AppVersion;
                request.villarequest.lang = language.Code();
                request.villarequest.mobileosversion = AppVersion;
                request.villarequest.sessionid = userSessionId;
                request.villarequest.userid = userId;
                request.villarequest.vendorid = GetVendorId(segment);

                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "hhvillamodify", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ApplicationResponse model = CustomJsonConvertor.DeserializeObject<ApplicationResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.Responsecode) && model.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<ApplicationResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                        return new ServiceResponse<ApplicationResponse>(null, false, model.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                    return new ServiceResponse<ApplicationResponse>(null, false, response.StatusDescription);
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ApplicationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<DownloadFileResponse> DownloadFile(DownloadFileRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.attachrequest.appidentifier = segment.Identifier();
                request.attachrequest.appversion = AppVersion;
                request.attachrequest.lang = language.Code();
                request.attachrequest.mobileosversion = AppVersion;
                request.attachrequest.sessionid = userSessionId;
                request.attachrequest.userid = userId;
                request.attachrequest.vendorid = GetVendorId(segment);

                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "hhvillaattachdownload", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    DownloadFileResponse model = CustomJsonConvertor.DeserializeObject<DownloadFileResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.responsecode) && model.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<DownloadFileResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                        return new ServiceResponse<DownloadFileResponse>(null, false, model.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                    return new ServiceResponse<DownloadFileResponse>(null, false, response.StatusDescription);
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<DownloadFileResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<UploadFileResponse> UploadFile(UploadFileRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.attachrequest.appidentifier = segment.Identifier();
                request.attachrequest.appversion = AppVersion;
                request.attachrequest.lang = language.Code();
                request.attachrequest.mobileosversion = AppVersion;
                request.attachrequest.sessionid = userSessionId;
                request.attachrequest.userid = userId;
                request.attachrequest.vendorid = GetVendorId(segment);

                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "hhvillaattachupload", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UploadFileResponse model = CustomJsonConvertor.DeserializeObject<UploadFileResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.responsecode) && model.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<UploadFileResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                        return new ServiceResponse<UploadFileResponse>(null, false, model.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                    return new ServiceResponse<UploadFileResponse>(null, false, response.StatusDescription);
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<UploadFileResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<ICADetailsResponse> GetICADetails(ICADetailsRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.lang = language.Code();
                request.mobileosversion = AppVersion;
                request.sessionid = userSessionId;
                request.userid = userId;
                request.vendorid = GetVendorId(segment);

                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "hhvillaicadetails", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ICADetailsResponse model = CustomJsonConvertor.DeserializeObject<ICADetailsResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.responsecode) && model.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<ICADetailsResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                        return new ServiceResponse<ICADetailsResponse>(null, false, model.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                    return new ServiceResponse<ICADetailsResponse>(null, false, response.StatusDescription);
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ICADetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<CountryListResponse> GetCountryList(CountryListRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.countrylistinput = new Countrylistinput
                {
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang= language.Code(),
                    mobileosversion = AppVersion,
                    sessionid = userSessionId,
                    userid = userId,
                    vendorid = GetVendorId(segment),
                };

                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "hhvillaicacountrylist", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    CountryListResponse model = CustomJsonConvertor.DeserializeObject<CountryListResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.responsecode) && model.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<CountryListResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                        return new ServiceResponse<CountryListResponse>(null, false, model.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception(string.Format(errorMessage, response.StatusCode, response.Content, response.StatusDescription)), this);
                    return new ServiceResponse<CountryListResponse>(null, false, response.StatusDescription);
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<CountryListResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
