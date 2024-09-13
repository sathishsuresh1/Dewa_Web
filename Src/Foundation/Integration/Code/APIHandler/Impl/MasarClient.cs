using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Common;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Newtonsoft.Json;
using RestSharp;
using Sitecore.Globalization;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{

    using @MasarRequest = Models.Request.Masar;
    using @MasarResponse = Models.Response.Masar;

    [Service(typeof(IMasarClient), Lifetime = Lifetime.Transient)]
    public class MasarClient : BaseApiDewaGateway, IMasarClient
    {

        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV4_ApiUrl}";
        private string BaseLoginApiUrl => $"{ApiBaseConfig.SmartCustomerV3_LoginApiUrl}";
        private string MerchantId => $"{ApiBaseConfig.SmartCustomerV4_MerchantId}";
        private string MerchantPassword => $"{ApiBaseConfig.SmartCustomerV4_MerchantPass}";

        private string VendorID => $"{ApiBaseConfig.CVI_Vendor_ID_DESKTOP}";

        EncryptDecrypt _encryption = new EncryptDecrypt();
        public ServiceResponse<@MasarResponse.MasarDropDownBaseResponse> GetMasarDropDownData(@MasarRequest.MasarDropdownBaseRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.dropdowninputs.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "supplierdropdown", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarDropDownBaseResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarDropDownBaseResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarDropDownBaseResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarDropDownBaseResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarDropDownBaseResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarDropDownBaseResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }


        public ServiceResponse<MasarOTPResponse> MasarSendOtp(MasarOTPRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {

                request.otpinput.lang = language.Code();

                if (!string.IsNullOrEmpty(request.otpinput.reference))
                    request.otpinput.reference = _encryption.DecryptText(request.otpinput.reference, MasarConfig._zStr);

                /*
                                if (!string.IsNullOrEmpty(request.otpinput.inputid))
                                {
                                    if (!string.IsNullOrEmpty(request.otpinput.email) && request.otpinput.email.IndexOf("*") > -1)
                                        request.otpinput.email = string.Empty;

                                    if (!string.IsNullOrEmpty(request.otpinput.mobile) && request.otpinput.mobile.IndexOf("*") > -1)
                                        request.otpinput.mobile = string.Empty;
                                }
                */

                IRestResponse response = DewaApiExecute(BaseApiUrl, "cviverifyotp", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarOTPResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarOTPResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        if (!string.IsNullOrEmpty(_Response.referencenumber))
                            _Response.referencenumber = _encryption.EncryptText(_Response.referencenumber, MasarConfig._zStr);

                        return new ServiceResponse<@MasarResponse.MasarOTPResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarOTPResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarOTPResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarOTPResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<MasarUserRegistrationResponse> ScrapRegistration(MasarUserRegistrationRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {

            try
            {

                request.customerinputs.lang = language.Code();

                IRestResponse response = DewaApiExecute(BaseApiUrl, "registercustomernb", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarUserRegistrationResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarUserRegistrationResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarUserRegistrationResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarUserRegistrationResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarUserRegistrationResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarUserRegistrationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);

        }


        public ServiceResponse<MasarAttachmentResponse> AddMasarAttachments(MasarAttachmentRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.attachmentinputs.reference))
                    request.attachmentinputs.reference = _encryption.DecryptText(request.attachmentinputs.reference, MasarConfig._zStr);

                IRestResponse response = DewaApiExecute(BaseApiUrl, !string.IsNullOrEmpty(request.attachmentinputs.sessionid) ? "uploadloginattachmentnb" : "uploadattachmentnb", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarAttachmentResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarAttachmentResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarAttachmentResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarAttachmentResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarAttachmentResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarAttachmentResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<MasarLoginResponse> MasarLogin(MasarLoginRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.getloginsessionrequest.merchantid = MerchantId;
                request.getloginsessionrequest.merchantpassword = MerchantPassword;
                request.getloginsessionrequest.lang = language.Code();
                request.getloginsessionrequest.password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.getloginsessionrequest.password));


                IRestResponse response = DewaApiExecute(BaseLoginApiUrl, "scrap", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarLoginResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarLoginResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarLoginResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarLoginResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarLoginResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarLoginResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }


        public ServiceResponse<MasarCreateUserCredentialResponse> MasarCreateUserCredential(MasarCreateUserCredentialRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.newuserinputs.reference))
                    request.newuserinputs.reference = _encryption.DecryptText(request.newuserinputs.reference, MasarConfig._zStr);

                request.newuserinputs.lang = language.Code();
                request.newuserinputs.password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.newuserinputs.password));


                IRestResponse response = DewaApiExecute(BaseApiUrl, "newusernb", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarCreateUserCredentialResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarCreateUserCredentialResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarCreateUserCredentialResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarCreateUserCredentialResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarCreateUserCredentialResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarCreateUserCredentialResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }


        public ServiceResponse<MasarGetMaskedEmailNMobileResponse> MasarGetMaskedEmailNPhone(MasarGetMaskedEmailNMobileRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {

                IRestResponse response = DewaApiExecute(BaseApiUrl, "readotpnb", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarGetMaskedEmailNMobileResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarGetMaskedEmailNMobileResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        if (!string.IsNullOrEmpty(_Response.referencenumber))
                            _Response.referencenumber = _encryption.EncryptText(_Response.referencenumber, MasarConfig._zStr);

                        return new ServiceResponse<@MasarResponse.MasarGetMaskedEmailNMobileResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarGetMaskedEmailNMobileResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarGetMaskedEmailNMobileResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarGetMaskedEmailNMobileResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userSessionId"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        public ServiceResponse<MasarBankResponse> GetBankList(string userId, string userSessionId, string requestNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                MasarBankRequest request = new MasarBankRequest();
                request.bankdisplayreq.sessionid = userSessionId;
                request.bankdisplayreq.userid = userId;
                request.bankdisplayreq.lang = language.Code();
                request.bankdisplayreq.requestnumber = requestNumber;
                IRestResponse response = DewaApiExecute(BaseApiUrl, "getsdbankdisplay", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MasarBankResponse _Response = CustomJsonConvertor.DeserializeObject<MasarBankResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<MasarBankResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<MasarBankResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<MasarBankResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<MasarBankResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        public ServiceResponse<CommonResponse> UpdatePassword(MasarChangePasswordRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.changepwdinput.lang = language.Code();
                request.changepwdinput.vendorid = VendorID;

                IRestResponse response = DewaApiExecute(BaseApiUrl, "changepwdscrapmsc", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    CommonResponse _Response = CustomJsonConvertor.DeserializeObject<CommonResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<CommonResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<CommonResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<CommonResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<CommonResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<AddBankResponse> AddBank(MasarAddBankRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.addbankinputs.lang = language.Code();

                IRestResponse response = DewaApiExecute(BaseApiUrl, "addbanknb", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    AddBankResponse _Response = CustomJsonConvertor.DeserializeObject<AddBankResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<AddBankResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<AddBankResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<AddBankResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<AddBankResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        public ServiceResponse<MasarForgetPasswordResponse> MasarForgotPassword(MasarForgetPasswordRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.resetcredentialdetails.reference))
                    request.resetcredentialdetails.reference = _encryption.DecryptText(request.resetcredentialdetails.reference, MasarConfig._zStr);

                request.resetcredentialdetails.lang = language.Code();
                request.resetcredentialdetails.password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.resetcredentialdetails.password));
                request.resetcredentialdetails.vendorid = VendorID;

                IRestResponse response = DewaApiExecute(BaseApiUrl, "setnonbillingresetpassword", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarForgetPasswordResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarForgetPasswordResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarForgetPasswordResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarForgetPasswordResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    //return new ServiceResponse<@MasarResponse.MasarForgetPasswordResponse>(null, false, $"response value:  '{JsonConvert.SerializeObject(response)}'");
                    return new ServiceResponse<@MasarResponse.MasarForgetPasswordResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarForgetPasswordResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<MasarDecryptGUIDResponse> MasarCreateUserDecryptGUID(MasarDecryptGUIDRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.decryptdetails.lang = language.Code();


                IRestResponse response = DewaApiExecute(BaseApiUrl, "getcvipassworddecrypt", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarDecryptGUIDResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarDecryptGUIDResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        if (!string.IsNullOrEmpty(_Response.referencenumber))
                            _Response.referencenumber = _encryption.EncryptText(_Response.referencenumber, MasarConfig._zStr);

                        return new ServiceResponse<@MasarResponse.MasarDecryptGUIDResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarDecryptGUIDResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarDecryptGUIDResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarDecryptGUIDResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<MasarProfileFetchResponse> MasarProfileFetch(MasarRequest.MasarProfileFetchRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {

            try
            {
                IRestResponse response = DewaApiExecute(BaseApiUrl, "customerdetailsnb", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarProfileFetchResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarProfileFetchResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarProfileFetchResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarProfileFetchResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarProfileFetchResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarProfileFetchResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);

        }


        public ServiceResponse<MasarForgotUserNameResponse> MasarForgotUserName(MasarForgotUserNameRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.forgotiddetails.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "getnonbillingforgotuserid", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarForgotUserNameResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarForgotUserNameResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarForgotUserNameResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarForgotUserNameResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarForgotUserNameResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarForgotUserNameResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }


        public ServiceResponse<MasarTrackApplicationResponse> MasarTrackApplication(MasarTrackApplicationRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.trackinputs.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "trackcustomernb", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarTrackApplicationResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarTrackApplicationResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarTrackApplicationResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarTrackApplicationResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarTrackApplicationResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarTrackApplicationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }


        public ServiceResponse<MasarTrackMiscellaneousResponse> MasarMiscellaneousTrackApplication(MasarTrackMiscellaneousRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.trackinput.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "trackcustomersd", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarTrackMiscellaneousResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarTrackMiscellaneousResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarTrackMiscellaneousResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarTrackMiscellaneousResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarTrackMiscellaneousResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarTrackMiscellaneousResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<MasarFetchICAResponse> MasarFetchICA(MasarFetchICARequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.icainputs.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "supplieremiratesid", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarFetchICAResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarFetchICAResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarFetchICAResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarFetchICAResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarFetchICAResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarFetchICAResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);

        }

        public ServiceResponse<MasarFetchDEDResponse> MasarFetchDED(MasarFetchDEDRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.tradelicenseinput.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "suppliertradelicense", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.MasarFetchDEDResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.MasarFetchDEDResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.MasarFetchDEDResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.MasarFetchDEDResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.MasarFetchDEDResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.MasarFetchDEDResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);

        }


        public ServiceResponse<ReadOtpNbResponse> MasarReadOtpNB(ReadOtpNbRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                IRestResponse response = DewaApiExecute(BaseApiUrl, "readotpnb", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @MasarResponse.ReadOtpNbResponse _Response = CustomJsonConvertor.DeserializeObject<@MasarResponse.ReadOtpNbResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@MasarResponse.ReadOtpNbResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@MasarResponse.ReadOtpNbResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@MasarResponse.ReadOtpNbResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@MasarResponse.ReadOtpNbResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }


        public ServiceResponse<MasarCancelBankResponse> CancelBankRequest(MasarCancelBankRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.bankdeletereq.lang = language.Code();

                IRestResponse response = DewaApiExecute(BaseApiUrl, "sdbankdelete", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MasarCancelBankResponse _Response = CustomJsonConvertor.DeserializeObject<MasarCancelBankResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<MasarCancelBankResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<MasarCancelBankResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<MasarCancelBankResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<MasarCancelBankResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<MasarDisplayBankResponse> DisplayBankRequest(MasarDisplayBankRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.bankdisplayreq.lang = language.Code();

                IRestResponse response = DewaApiExecute(BaseApiUrl, "getsdbankdisplay", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MasarDisplayBankResponse _Response = CustomJsonConvertor.DeserializeObject<MasarDisplayBankResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<MasarDisplayBankResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<MasarDisplayBankResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<MasarDisplayBankResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<MasarDisplayBankResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }


        public ServiceResponse<MasarTrackInputResponse> FetchTrackApplicationData(MasarTrackInputRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.trackinput.lang = language.Code();

                IRestResponse response = DewaApiExecute(BaseApiUrl, "gettrackrequestnonbilling", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MasarTrackInputResponse _Response = CustomJsonConvertor.DeserializeObject<MasarTrackInputResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<MasarTrackInputResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<MasarTrackInputResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<MasarTrackInputResponse>(null, false, Translate.Text("Webservice Error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<MasarTrackInputResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }


    public static class MasarConfig
    {
        public static string ScrapSales = "S";
        public static string Miscellaneous = "M";
        public static string CreateProfile = "C";
        public static string ResubmitProfile = "R";
        public static string UpdateProfile = "U";
        public static string Individual = "1";
        public static string Organization = "2";
        public static string NonBilling = "NONB";
        public static string Mode = "S";
        public static string ModeVerify = "V";
        public static string IdTYpe = "U";
        public static string Company = "0003";
        public static string Mr = "0002";
        public static string ScrapSaleDashboardURL = "/scrap-sale/myaccount/dashboard";
        public static string ElectricMeterTesting = "/consumer/my-account/miscellaneous/electricity-meter-testing";
        public static string ElectricMeterTestingDEWAProjects = "/consumer/my-account/miscellaneous/electricity-meter-testing-for-dewa-projects";
        public static string TransformerOilTesting = "/consumer/my-account/miscellaneous/oil-testing";
        public static string JointerCertificationFormURL = "/consumer/my-account/miscellaneous/jointer-testing";
        public static string MiscellaneousProfile = "/miscellaneous/my-profile";
        public static string MiscellaneousBank = "/miscellaneous/bank-list";
        public static string MiscellaneousChangePassword = "/miscellaneous/change-password";
        public static string ScrapType = "Scrap";
        public static string MiscellaneousType = "Miscellaneous";
        public static string TestingServices = "T";
        public static string JointerCertification = "J";
        public static string _zStr = "HeroKool";
    }


    public class MasarSendOtpRequest
    {
        public string mobile { get; set; }
        public string email { get; set; }
        public string bpno { get; set; }
        public string type { get; set; }
        public string Otp { get; set; }
        public string mode { get; set; }
        public string reqId { get; set; }
        public string prtype { get; set; }
        public string option { get; set; }
        public string emiratesId { get; set; }
        public string bpcategory { get; set; }
        public string inputId { get; set; }
        public string idType { get; set; }
    }


    public class EncryptDecrypt
    {
        public string EncryptText(string originalString, string key)
        {

            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(key);
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException
                       ("The string which needs to be encrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        public string EncryptTextURL(string originalString, string key)
        {

            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(key);
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException
                       ("The string which needs to be encrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return HttpUtility.UrlEncode(Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length));
        }


        public string DecryptText(string cryptedString, string key)
        {
            StreamReader reader;
            try
            {
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes(key);
                if (String.IsNullOrEmpty(cryptedString))
                {
                    throw new ArgumentNullException
                       ("The string which needs to be decrypted can not be null.");
                }
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream
                        (Convert.FromBase64String(cryptedString));
                CryptoStream cryptoStream = new CryptoStream(memoryStream,
                    cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
                reader = new StreamReader(cryptoStream);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {

            }
            return string.Empty;

        }

    }


}
