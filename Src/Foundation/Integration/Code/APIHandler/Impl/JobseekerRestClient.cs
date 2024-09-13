using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Common;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using System.Web.Configuration;
using DEWAXP.Foundation.Integration.JobSeekerSvc;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IJobseekerRestClient), Lifetime = Lifetime.Transient)]
    public class JobseekerRestClient : BaseApiDewaGateway, IJobseekerRestClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.JobseekerV2_ApiUrl}";

        private string UsersApiUrl => $"{ApiBaseConfig.UsersV3_ApiUrl}";

        public ServiceResponse<OTPResponse> VerifyOtp(OTPRequest request, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                IRestResponse response = DewaApiExecute(BaseApiUrl, "otp", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    OTPResponse _Response = CustomJsonConvertor.DeserializeObject<OTPResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<OTPResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<OTPResponse>(_Response, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<OTPResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<OTPResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<CommonResponse> UnlockAccount(UnlockAccountRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                IRestResponse response = DewaApiExecute(UsersApiUrl, "unlock/jobseeker", request, Method.POST, null);
                //SmartUserSubmit(SmartCustomerConstant.UNLOCKUSERID, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    CommonResponse _Response = CustomJsonConvertor.DeserializeObject<CommonResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<CommonResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<CommonResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<CommonResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<CommonResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        public ServiceResponse<LoginResponse> LoginUser(LoginRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.getloginsessionrequest.lang = language.Code();
                request.getloginsessionrequest.appidentifier = segment.Identifier();
                request.getloginsessionrequest.appversion = AppVersion;
                request.getloginsessionrequest.merchantid = WebConfigurationManager.AppSettings["Jobseeker_New_UserName"];
                request.getloginsessionrequest.merchantpassword = WebConfigurationManager.AppSettings["Jobseeker_New_Password"];

                IRestResponse response = DewaApiExecute(UsersApiUrl, "login/jobseeker", request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    LoginResponse _Response = CustomJsonConvertor.DeserializeObject<LoginResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.ResponseCode) && _Response.ResponseCode.Equals("000"))
                    {
                        return new ServiceResponse<LoginResponse>(_Response);
                    }
                    else if (_Response != null && !string.IsNullOrWhiteSpace(_Response.ResponseCode) && _Response.ResponseCode.Equals("117"))
                    {
                        return new ServiceResponse<LoginResponse>(_Response, false, ErrorMessages.InvalidCredential);
                    }
                    else if (_Response != null && !string.IsNullOrWhiteSpace(_Response.ResponseCode) && _Response.ResponseCode.Equals("116"))
                    {
                        return new ServiceResponse<LoginResponse>(_Response, false, ErrorMessages.AccountLocked);
                    }
                    else
                    {
                        return new ServiceResponse<LoginResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<LoginResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<LoginResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);

        }

        public ServiceResponse<CommonResponse> ForgotUseridPwd(ForgotPasswordRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                IRestResponse response = DewaApiExecute(UsersApiUrl, "forgotpass/jobseeker", request, Method.POST, null);                

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    CommonResponse _Response = CustomJsonConvertor.DeserializeObject<CommonResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<CommonResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<CommonResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<CommonResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<CommonResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<ForgotUserNameResponse> ForgotUserName(ForgotUserNameRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.vendorid = GetJobSeekerVendorId(segment); // WebConfigurationManager.AppSettings["RammasMerchantId"];
                IRestResponse response = DewaApiExecute(UsersApiUrl, "forgotuserid/jobseeker", request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ForgotUserNameResponse _Response = CustomJsonConvertor.DeserializeObject<ForgotUserNameResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.errorcode) && _Response.errorcode.Equals("0"))
                    {
                        return new ServiceResponse<ForgotUserNameResponse>(_Response);
                    }                   
                    else
                    {
                        return new ServiceResponse<ForgotUserNameResponse>(_Response, false, _Response.errormessage);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<ForgotUserNameResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ForgotUserNameResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
