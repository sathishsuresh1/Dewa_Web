
namespace DEWAXP.Foundation.Integration.Impl.SmartCustomerV3Svc
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Helpers;
    using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
    using DEWAXP.Foundation.Integration.Responses;
    using RestSharp;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Configuration;
    using System;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVSmartCharger;
    using DEWAXP.Foundation.Integration.Responses.SmartCustomer;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
    using DEWAXP.Foundation.Integration.Responses.SmartConsultant;
    using DEWAXP.Foundation.Integration.Impl.SmartConsultant;
    using DEWAXP.Foundation.Integration.Requests;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Common;
    using DEWAXP.Foundation.Integration.Responses.Emirates;
    using DEWAXP.Foundation.DI;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Payment;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Payment;


    /// <summary>
    ///  Defines the <see cref="SmartCustomerClient" />.
    /// </summary>
    [Service(typeof(ISmartCustomerClient),Lifetime =Lifetime.Transient)]
    public class SmartCustomerClient : BaseDewaGateway, ISmartCustomerClient
    {
        internal string DEWASmartGovtPortURL { get; set; } = WebConfigurationManager.AppSettings["RestAPI_Smart_Customer_V3"];
        internal string DEWASmartUsersURL { get; set; } = WebConfigurationManager.AppSettings["DEWASMARTUSER"];

        public ServiceResponse<AccountDetails[]> GetCAList(string userId, string sessionId, string checkMoveOut, string ServiceFlag, bool includeInactive = false, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                if (!includeInactive)
                {
                    #region Getting Active Accounts
                    var request = new Dictionary<string, string>() { { "sessionid", sessionId }, { "userid", userId }, { "checknotification", checkMoveOut }, { "language", language.Code() } };
                    IRestResponse  response = SmartCustomerSubmit(SmartCustomerConstant.GET_ACTIVE_CONTRACT_ACCOUNTS, request, Method.POST, null);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ContractAccountListResponse _Response = CustomJsonConvertor.DeserializeObject<ContractAccountListResponse>(response.Content);
                        if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responseCode) && _Response.responseCode.Equals("000"))
                        {
                            string activeCode = Convert.ToInt32(AccountClassification.Active).ToString("00");
                            var payload = _Response.ContractAccounts.Select(acc => new AccountDetails
                            {
                                AccountNumber = acc.AccountNumber,
                                PremiseNumber = acc.PremiseNumber,
                                CustomerPremiseNumber = acc.LegacyAccountNumber,
                                BusinessPartnerNumber = acc.BusinessPartnerNumber,
                                BillingClassCode = acc.BillingClassCode,
                                Category = acc.Category,
                                AccountName = acc.Name,
                                NickName = acc.Nickname,
                                BPName = acc.BPName,
                                IsActive = acc.IsActive,
                                PhotoIndicator = acc.PhotoIndicator,
                                CustomerType = acc.CustomerType,
                                NotificationNumber = acc.NotificationNumber,
                                AccountCategory = acc.AccountCategory,
                                PremiseType = acc.PremiseType,
                                Street = acc.Street,
                                Location = acc.Location,
                                AccountStatusCode = activeCode,
                                POD = acc.POD,
                                Medical = acc.Medical,
                                Senior = acc.Senior,
                                XCordinate = acc.XCordinate,
                                YCordinate = acc.YCordinate,
                                Isexpocustomer = _Response.isexpocustomer
                            }).ToArray();
                            return new ServiceResponse<AccountDetails[]>(payload);
                        }
                        else
                        {
                            return new ServiceResponse<AccountDetails[]>(null, false, _Response.responseMessage);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Getting Active and Inactive Accounts
                    var request = new Dictionary<string, string>() { { "sessionid", sessionId }, { "userid", userId }, { "serviceflag", ServiceFlag }, { "language", language.Code() } };
                    IRestResponse  response = SmartCustomerSubmit(SmartCustomerConstant.GET_ALL_CONTRACT_ACCOUNTS, request, Method.POST, null);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        DetailedContractAccountListResponse _Response = CustomJsonConvertor.DeserializeObject<DetailedContractAccountListResponse>(response.Content);
                        if (_Response.responseCode == "000")
                        {
                            _Response.ContractAccounts.Where(c => c.AccountStatusCode == "00").ToList().ForEach(cc => cc.IsActive = true);
                            return new ServiceResponse<AccountDetails[]>(_Response.ContractAccounts.ToArray());
                        }
                        else
                        {
                            return new ServiceResponse<AccountDetails[]>(null, false, _Response.responseMessage);
                        }
                    }
                    #endregion
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<AccountDetails[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);

        }

        public ServiceResponse<SmartMeterResponse> GetSmartMeterDetails(SmartMeterRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.smartmeterinputs.lang = language.Code();
                IRestResponse  response = SmartCustomerSubmit(SmartCustomerConstant.GET_SMART_METER_RESPONSE, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SmartMeterResponse _Response = CustomJsonConvertor.DeserializeObject<SmartMeterResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<SmartMeterResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<SmartMeterResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<SmartMeterResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<SmartMeterResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }


        #region EV Smart Charger
        public ServiceResponse<OneTimeChargeResponse> EVSmartCharger(OneTimeChargeRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.chargein.lang = language.Code();
                request.chargein.vendorid = GetVendorId(segment);
                IRestResponse  response = SmartCustomerSubmit(SmartCustomerConstant.EVSMARTCHARGER, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    OneTimeChargeResponse _Response = CustomJsonConvertor.DeserializeObject<OneTimeChargeResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<OneTimeChargeResponse>(_Response);
                    }
                    else if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("399") && _Response.Maxotp)
                    {
                        return new ServiceResponse<OneTimeChargeResponse>(_Response, false, _Response.Description);
                    }
                    else
                    {
                        return new ServiceResponse<OneTimeChargeResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<OneTimeChargeResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<OneTimeChargeResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

        #endregion

        public ServiceResponse<BillHistoryResponse> GetBillPaymentHistory(BillHistoryRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.lang = language.Code();
                IRestResponse  response = SmartCustomerSubmit(SmartCustomerConstant.BILLPAYMENTHISTORY, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    BillHistoryResponse _Response = CustomJsonConvertor.DeserializeObject<BillHistoryResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && 
                        (_Response.Responsecode.Equals("000")|| _Response.Responsecode.Equals("105")))
                    {
                        return new ServiceResponse<BillHistoryResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<BillHistoryResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<BillHistoryResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<BillHistoryResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        #region Get Infrastructure NOC list
        public ServiceResponse<InfrastructureNocResponse> GetInfrastructureNocList(InfrastructureNocRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.lang = language.Code();
                request.userid = userId;
                request.sessionid = sessionId;
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.mobileosversion = AppVersion;
                request.vendorid = GetVendorId(segment);

                IRestResponse  response = SmartCustomerSubmit(SmartCustomerConstant.INFRASTRUCTURE_NOC_LIST, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    InfrastructureNocResponse _Response = CustomJsonConvertor.DeserializeObject<InfrastructureNocResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<InfrastructureNocResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<InfrastructureNocResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<InfrastructureNocResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<InfrastructureNocResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        #endregion

        #region Get Infrastructure NOC Details
        public ServiceResponse<InfrastructureNocResponse> GetInfrastructureNocDetails(InfrastructureNocRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.userid = userId;
                request.sessionid = sessionId;
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.mobileosversion = AppVersion;
                request.vendorid = GetVendorId(segment);
                request.lang = language.Code();

                IRestResponse  response = SmartCustomerSubmit(SmartCustomerConstant.INFRASTRUCTURE_NOC_DETAILS, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    InfrastructureNocResponse _Response = CustomJsonConvertor.DeserializeObject<InfrastructureNocResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<InfrastructureNocResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<InfrastructureNocResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<InfrastructureNocResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<InfrastructureNocResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        #endregion

        #region Get Infrastructure Noc Work Types
        public ServiceResponse<InfrastructureNocWorkTypeResponse> GeWorkTypes(InfrastructureNocRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {

                request.userid = userId;
                request.sessionid = sessionId;
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.mobileosversion = AppVersion;
                request.vendorid = GetVendorId(segment);
                request.lang = language.Code();

                IRestResponse  response = SmartCustomerSubmit(SmartCustomerConstant.INFRASTRUCTURE_NOC_WORKTYPES, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    InfrastructureNocWorkTypeResponse _Response = CustomJsonConvertor.DeserializeObject<InfrastructureNocWorkTypeResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<InfrastructureNocWorkTypeResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<InfrastructureNocWorkTypeResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<InfrastructureNocWorkTypeResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<InfrastructureNocWorkTypeResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        #endregion

        #region Infrastructure NOC Submit 
        public ServiceResponse<InfrastructureNocSubmitReponse> SubmitNewNocRequest(InfraNocSubmitRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.InfraNocRequest.lang = language.Code();
                request.InfraNocRequest.userid = userId;
                request.InfraNocRequest.sessionid = sessionId;
                request.InfraNocRequest.appidentifier = segment.Identifier();
                request.InfraNocRequest.appversion = AppVersion;
                request.InfraNocRequest.mobileosversion = AppVersion;
                request.InfraNocRequest.vendorid = GetVendorId(segment);

                string methodName = SmartCustomerConstant.INFRASTRUCTURE_NOC_SUBMIT;
                if (!string.IsNullOrWhiteSpace(request.InfraNocRequest.transactionid))
                    methodName = SmartCustomerConstant.INFRASTRUCTURE_NOC_RESUBMIT;

                IRestResponse  response = SmartCustomerSubmit(methodName, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    InfrastructureNocSubmitReponse _Response = CustomJsonConvertor.DeserializeObject<InfrastructureNocSubmitReponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<InfrastructureNocSubmitReponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<InfrastructureNocSubmitReponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<InfrastructureNocSubmitReponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<InfrastructureNocSubmitReponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        #endregion

        #region Infrasture Download attachment
        public ServiceResponse<InfrastructureNocAttachmentResponse> DownloadFile(InfrastructureNocRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.userid = userId;
                request.sessionid = sessionId;
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.mobileosversion = AppVersion;
                request.vendorid = GetVendorId(segment);
                request.lang = language.Code();

                IRestResponse  response = SmartCustomerSubmit(SmartCustomerConstant.INFRASTRUCTURE_NOC_DOWNLOAD_FILE, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    InfrastructureNocAttachmentResponse _Response = CustomJsonConvertor.DeserializeObject<InfrastructureNocAttachmentResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<InfrastructureNocAttachmentResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<InfrastructureNocAttachmentResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<InfrastructureNocAttachmentResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<InfrastructureNocAttachmentResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        #endregion

        #region Infrastructure get status/history details
        public ServiceResponse<InfrastructureNocStatusResponse> GetStatusDetails(InfrastructureNocRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.userid = userId;
                request.sessionid = sessionId;
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.mobileosversion = AppVersion;
                request.vendorid = GetVendorId(segment);
                request.lang = language.Code();

                IRestResponse  response = SmartCustomerSubmit(SmartCustomerConstant.INFRASTRUCTURE_NOC_STATUS, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    InfrastructureNocStatusResponse _Response = CustomJsonConvertor.DeserializeObject<InfrastructureNocStatusResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<InfrastructureNocStatusResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<InfrastructureNocStatusResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<InfrastructureNocStatusResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<InfrastructureNocStatusResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        #endregion

        #region Infrastructure get Infranoc ActiveAccount
        public ServiceResponse<InfranocActiveAccountResponse> InfranocActiveAccount(InfranocActiveAccountRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.userid = userId;
                request.sessionid = sessionId;
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.mobileosversion = AppVersion;
                request.vendorid = GetVendorId(segment);
                request.lang = language.Code();

                IRestResponse  response = SmartCustomerSubmit("infranocactiveaccount", request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    InfranocActiveAccountResponse _Response = CustomJsonConvertor.DeserializeObject<InfranocActiveAccountResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<InfranocActiveAccountResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<InfranocActiveAccountResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<InfranocActiveAccountResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<InfranocActiveAccountResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
        #endregion

        public ServiceResponse<EmiratesIDIntegrationResponse> GetEmiratesIDdetails(EmiratesIDIntegrationRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                IRestResponse  response = SmartCustomerSubmit(SmartCustomerConstant.GET_EMIRATESID_DETAILS, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EmiratesIDIntegrationResponse _Response = CustomJsonConvertor.DeserializeObject<EmiratesIDIntegrationResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<EmiratesIDIntegrationResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<EmiratesIDIntegrationResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<EmiratesIDIntegrationResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<EmiratesIDIntegrationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<CommonResponse> ForgotUseridPwd(ForgotPasswordRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                IRestResponse  response = SmartUserSubmit(SmartCustomerConstant.FORGOTUSERIDPWD, request, Method.POST, null);

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

        public ServiceResponse<CommonResponse> UnlockAccount(UnlockAccountRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                IRestResponse  response = SmartUserSubmit(SmartCustomerConstant.UNLOCKUSERID, request, Method.POST, null);

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
                request.getloginsessionrequest.merchantid = GetMerchantId(segment);
                request.getloginsessionrequest.merchantpassword = GetMerchantPassword(segment);
                IRestResponse  response = SmartUserSubmit(SmartCustomerConstant.LOGIN_USER, request, Method.POST, null);

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
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<LoginResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<LoginResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<UAEPGSResponse> UAEPGSList(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                UAEPGSRequest request = new UAEPGSRequest
                {
                    lang = language.Code(),
                    vendorid = GetVendorId(segment)
                };

                IRestResponse response = SmartCustomerSubmit(SmartCustomerConstant.UAEPGSLIST, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UAEPGSResponse _Response = CustomJsonConvertor.DeserializeObject<UAEPGSResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<UAEPGSResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<UAEPGSResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<UAEPGSResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<UAEPGSResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        /// <summary>
        /// The SmartCustomerSubmit.
        /// </summary>
        /// <param name="methodname">The methodname<see cref="string"/>.</param>
        /// <param name="requestbody">The requestbody<see cref="object"/>.</param>
        /// <param name="method">The method<see cref="Method"/>.</param>
        /// <param name="Querystring_Array">The Querystring_Array<see cref="Dictionary{string, string}"/>.</param>
        /// <returns>The <see cref="RestResponse"/>.</returns>
        public IRestResponse  SmartCustomerSubmit(string methodname, object requestbody, Method method = Method.POST, Dictionary<string, string> Querystring_Array = null)
        {
            RestRequest request = null;
            RestClient client = CreateClient();
            request = new RestRequest(methodname, method);
            request = CreateHeader(request);
            if (Querystring_Array != null)
            {
                request = CreateQueryString(request, Querystring_Array);
            }
            if (requestbody != null)
            {
                request.AddBody(requestbody);
            }
            IRestResponse  response = client.Execute(request);
            return response;
        }

        public IRestResponse  SmartUserSubmit(string methodname, object requestbody, Method method = Method.POST, Dictionary<string, string> Querystring_Array = null)
        {
            RestRequest request = null;
            RestClient client = CreateUserClient();
            request = new RestRequest(methodname, method);
            request = CreateHeader(request);
            if (Querystring_Array != null)
            {
                request = CreateQueryString(request, Querystring_Array);
            }
            if (requestbody != null)
            {
                request.AddBody(requestbody);
            }
            IRestResponse  response = client.Execute(request);
            return response;
        }
        private RestClient CreateUserClient()
        {
            return new RestClient(DEWASmartUsersURL);
        }

        /// <summary>
        /// The CreateClient.
        /// </summary>
        /// <returns>The <see cref="RestClient"/>.</returns>
        private RestClient CreateClient()
        {
            return new RestClient(DEWASmartGovtPortURL);
        }

        /// <summary>
        /// The CreateHeader.
        /// </summary>
        /// <param name="request">The request<see cref="RestRequest"/>.</param>
        /// <returns>The <see cref="RestRequest"/>.</returns>
        private RestRequest CreateHeader(RestRequest request)
        {
            request.AddHeader("Authorization", "Bearer " + OAuthToken.GetAccessToken());
            request.RequestFormat = DataFormat.Json;
            return request;
        }

        /// <summary>
        /// The CreateQueryString.
        /// </summary>
        /// <param name="request">The request<see cref="RestRequest"/>.</param>
        /// <param name="Querystring_Array">The Querystring_Array<see cref="Dictionary{string, string}"/>.</param>
        /// <returns>The <see cref="RestRequest"/>.</returns>
        private RestRequest CreateQueryString(RestRequest request, Dictionary<string, string> Querystring_Array)
        {
            Querystring_Array.ToList().ForEach
            (
                pair =>
                {
                    request.AddQueryParameter(pair.Key, pair.Value);
                }
            );
            return request;
        }
    }
}
