using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.Integration.CustomerSmartSalesSvc;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Logger;

namespace DEWAXP.Foundation.Integration.Impl.CustomerSmartSale
{
    [Service(typeof(ICustomerSmartSaleClient), Lifetime = Lifetime.Transient)]
    public class CustomerSmartSaleClient : BaseDewaGateway, ICustomerSmartSaleClient
    {
        public ServiceResponse<GetLoginSessionScrapCustomerResponse> GetLoginSessionScrapCustomer(GetLoginSessionScrapCustomer request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {

            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.merchantid = GetMerchantId(segment);
                    request.merchantpassword = GetMerchantPassword(segment);
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;


                    var response = client.GetLoginSessionScrapCustomer(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetLoginSessionScrapCustomerResponse>(response);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        var typedresponse = new ServiceResponse<GetLoginSessionScrapCustomerResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetLoginSessionScrapCustomerResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetLoginSessionScrapCustomerResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetLoginSessionScrapCustomerResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse GetScrapCustomerForgotUserID(GetScrapCustomerForgotUserID request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);

                    var response = client.GetScrapCustomerForgotUserID(request);
                    if (response != null && response.@return != null)
                    {
                        var _serializeResponse = response.@return.DeserializeAs<Responses.ScrapSale.ForgotUserID>();
                        return new ServiceResponse(Convert.ToBoolean(_serializeResponse.ResponseCode == "000"), _serializeResponse.Description);
                    }

                    return new ServiceResponse();
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse(false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }


        public ServiceResponse SetScrapCustomerPasswordReset(SetScrapCustomerPasswordReset request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);

                    var response = client.SetScrapCustomerPasswordReset(request);
                    if (response != null && response.@return != null)
                    {
                        var _serializeResponse = response.@return.DeserializeAs<Responses.ScrapSale.ResetCustomerPortalPassword>();
                        return new ServiceResponse(Convert.ToBoolean(_serializeResponse.ResponseCode == "000"), _serializeResponse.Description);
                    }

                    return new ServiceResponse();
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse(false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }


        public ServiceResponse SetScrapCustomerPasswordChange(SetScrapCustomerPasswordChange request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;


                    var response = client.SetScrapCustomerPasswordChange(request);

                    if (response != null && response.@return != null)
                    {
                        var _serializeResponse = response.@return.DeserializeAs<Responses.ScrapSale.ForgotUserID>();
                        return new ServiceResponse(Convert.ToBoolean(_serializeResponse.ResponseCode == "000"), _serializeResponse.Description);
                    }
                    return new ServiceResponse();
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse(false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }


        public ServiceResponse SetScrapCustomerNewPassword(SetScrapCustomerPasswordValidate request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;

                    var response = client.SetScrapCustomerPasswordValidate(request);

                    if (response != null && response.@return != null)
                    {
                        var _serializeResponse = response.@return.DeserializeAs<Responses.ScrapSale.PasswordValidate>();
                        return new ServiceResponse(Convert.ToBoolean(_serializeResponse.ResponseCode == "000"), _serializeResponse.Description);
                    }
                    return new ServiceResponse();
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse(false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }


        public ServiceResponse<GetTenderAdvertisementResponse> GetTenderAdvertisement(GetTenderAdvertisement request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);

                    var response = client.GetTenderAdvertisement(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetTenderAdvertisementResponse>(response);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        var typedresponse = new ServiceResponse<GetTenderAdvertisementResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetTenderAdvertisementResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetTenderAdvertisementResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetTenderAdvertisementResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<GetSSTenderPayStatusResponse> GetSSTenderPayStatus(GetSSTenderPayStatus request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);

                    var response = client.GetSSTenderPayStatus(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetSSTenderPayStatusResponse>(response);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        var typedresponse = new ServiceResponse<GetSSTenderPayStatusResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetSSTenderPayStatusResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetSSTenderPayStatusResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetSSTenderPayStatusResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<GetOpenTenderListResponse> GetOpenTenderList(GetOpenTenderList request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);

                    var response = client.GetOpenTenderList(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetOpenTenderListResponse>(response);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        var typedresponse = new ServiceResponse<GetOpenTenderListResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetOpenTenderListResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetOpenTenderListResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetOpenTenderListResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<GetOpenTenderListRequestedResponse> GetOpenTenderListRequested(GetOpenTenderListRequested request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);
                    var response = client.GetOpenTenderListRequested(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetOpenTenderListRequestedResponse>(response);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        var typedresponse = new ServiceResponse<GetOpenTenderListRequestedResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetOpenTenderListRequestedResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetOpenTenderListRequestedResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetOpenTenderListRequestedResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<GetTenderListPurchasedHistoryResponse> GetTenderListPurchasedHistory(GetTenderListPurchasedHistory request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);
                    var response = client.GetTenderListPurchasedHistory(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetTenderListPurchasedHistoryResponse>(response);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        var typedresponse = new ServiceResponse<GetTenderListPurchasedHistoryResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetTenderListPurchasedHistoryResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetTenderListPurchasedHistoryResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetTenderListPurchasedHistoryResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }


        public ServiceResponse<SetScrapRegistrationResponse> SetScrapRegistration(SetScrapRegistration request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    request.registrationinput.appidentifier = segment.Identifier();
                    request.registrationinput.lang = language.Code();
                    request.registrationinput.appversion = AppVersion;
                    request.registrationinput.mobileosversion = AppVersion;
                    request.registrationinput.vendorid = GetVendorId(segment);

                    var response = client.SetScrapRegistration(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        return new ServiceResponse<SetScrapRegistrationResponse>(response);
                    }
                    else if (response != null && response.@return != null && response.@return.responsecode == "105")
                    {

                        return new ServiceResponse<SetScrapRegistrationResponse>(response, true, response.@return.description);
                    }
                    return new ServiceResponse<SetScrapRegistrationResponse>(response, false, response.@return.description);
                }
            }
            catch (TimeoutException)
            {
                return new ServiceResponse<SetScrapRegistrationResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<SetScrapRegistrationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }



        }

        public ServiceResponse<GetRegistrationScrapCustomerDetailsResponse> GetRegistrationScrapCustomerDetails(GetRegistrationScrapCustomerDetails request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    request.appidentifier = segment.Identifier();
                    request.merchantid = GetMerchantId(segment);
                    request.merchantpassword = GetMerchantPassword(segment);
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);

                    var response = client.GetRegistrationScrapCustomerDetails(request);

                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        return new ServiceResponse<GetRegistrationScrapCustomerDetailsResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responsecode == "105")
                    {
                        return new ServiceResponse<GetRegistrationScrapCustomerDetailsResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetRegistrationScrapCustomerDetailsResponse>(response, false, response.@return.description);



                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetRegistrationScrapCustomerDetailsResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetRegistrationScrapCustomerDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<SetScrapCustomerAccountRegistrationResponse> SetScrapCustomerAccountRegistration(SetScrapCustomerAccountRegistration request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.merchantid = GetMerchantId(segment);
                    request.merchantpassword = GetMerchantPassword(segment);
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);
                    var response = client.SetScrapCustomerAccountRegistration(request);

                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        return new ServiceResponse<SetScrapCustomerAccountRegistrationResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responsecode == "105")
                    {
                        return new ServiceResponse<SetScrapCustomerAccountRegistrationResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<SetScrapCustomerAccountRegistrationResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<SetScrapCustomerAccountRegistrationResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<SetScrapCustomerAccountRegistrationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<SendVerifyScrapRegistrationCodeResponse> SendVerifyScrapRegistrationCode(SendVerifyScrapRegistrationCode request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    request.merchantid = GetMerchantId(segment);
                    request.merchantpassword = GetMerchantPassword(segment);
                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);

                    var response = client.SendVerifyScrapRegistrationCode(request);

                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        return new ServiceResponse<SendVerifyScrapRegistrationCodeResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responsecode == "105")
                    {
                        return new ServiceResponse<SendVerifyScrapRegistrationCodeResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<SendVerifyScrapRegistrationCodeResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<SendVerifyScrapRegistrationCodeResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<SendVerifyScrapRegistrationCodeResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        public ServiceResponse GetUserIDCheck(GetUserIDCheck request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);
                    var response = client.GetUserIDCheck(request);

                    if (response != null && response.@return != null)
                    {
                        var _serializeResponse = response.@return.DeserializeAs<Responses.ScrapSale.UserIDAvailability>();
                        return new ServiceResponse(Convert.ToBoolean(_serializeResponse.ResponseCode == "000"), _serializeResponse.Description);
                    }

                    return new ServiceResponse();

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse(false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }


        public ServiceResponse<GetLoginOpenTenderListResponse> GetLoginOpenTenderList(GetLoginOpenTenderList request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);
                    var response = client.GetLoginOpenTenderList(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetLoginOpenTenderListResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetLoginOpenTenderListResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetLoginOpenTenderListResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetLoginOpenTenderListResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetLoginOpenTenderListResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<GetTenderDocumentDownloadResponse> GetTenderDocumentDownload(GetTenderDocumentDownload request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);
                    GetTenderDocumentDownloadResponse response = client.GetTenderDocumentDownload(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetTenderDocumentDownloadResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetTenderDocumentDownloadResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetTenderDocumentDownloadResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetTenderDocumentDownloadResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetTenderDocumentDownloadResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<GetTenderReceiptResponse> GetTenderReceipt(GetTenderReceipt request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);
                    var response = client.GetTenderReceipt(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetTenderReceiptResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetTenderReceiptResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetTenderReceiptResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetTenderReceiptResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetTenderReceiptResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<GetTenderReferenceNumberResponse> GetTenderReferenceNumber(GetTenderReferenceNumber request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);
                    var response = client.GetTenderReferenceNumber(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetTenderReferenceNumberResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetTenderReferenceNumberResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetTenderReferenceNumberResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetTenderReferenceNumberResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetTenderReferenceNumberResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }


        public ServiceResponse<GetCountryHelpValuesResponse> GetCountryHelpValues(GetCountryHelpValues request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {

                    //request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    //request.appversion = AppVersion;
                    //request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);
                    var response = client.GetCountryHelpValues(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetCountryHelpValuesResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetCountryHelpValuesResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetCountryHelpValuesResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetCountryHelpValuesResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetCountryHelpValuesResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }


        #region Factory methods

        private CustomerScrapServicesClient CreateProxy()
        {
            var client = new CustomerScrapServicesClient(CreateBinding(), GetEndpointAddress("SmartSalesPort"));

            client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
            client.ChannelFactory.Endpoint.Behaviors.Add(new DewaApiCredentials());
            client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["RestAPI_Client_Id"], "Bearer " + OAuthToken.GetAccessToken()));
            client.ClientCredentials.UserName.UserName = BbUsername;
            client.ClientCredentials.UserName.Password = BbPassword;
            return client;
        }


        private CustomBinding CreateBinding()
        {
            var binding = new CustomBinding()
            {
                ReceiveTimeout = TimeSpan.FromMinutes(2),
                SendTimeout = TimeSpan.FromMinutes(2)
            };

            var security = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            security.IncludeTimestamp = true;
            security.LocalClientSettings.MaxClockSkew = new TimeSpan(0, 0, 10, 0);
            security.LocalServiceSettings.MaxClockSkew = new TimeSpan(0, 0, 10, 0);
            security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            security.SecurityHeaderLayout = SecurityHeaderLayout.Lax;
            security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;
            security.EnableUnsecuredResponse = true;
            security.AllowInsecureTransport = true;

            var encoding = new TextMessageEncodingBindingElement();
            encoding.MessageVersion = MessageVersion.Soap11;

            var transport = new HttpsTransportBindingElement();
            transport.MaxReceivedMessageSize = 20000000; // 20 megs

            binding.Elements.Add(security);
            binding.Elements.Add(new CustomTextMessageBindingElement());
            binding.Elements.Add(transport);


            return binding;
        }


        #endregion

        #region Phase-2

        #region GetBOQDisplay
        public ServiceResponse<GetBOQDisplayResponse> GetBOQDisplay(GetBOQDisplay request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    request.appidentifier = segment.Identifier();
                    request.appversion = AppVersion;
                    request.lang = language.Code();
                    request.mobileosversion = AppVersion;
                    request.sessionid = sessionId;
                    request.userid = userId;
                    request.vendorid = GetVendorId(segment);


                    var response = client.GetBOQDisplay(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetBOQDisplayResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetBOQDisplayResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetBOQDisplayResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetBOQDisplayResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetBOQDisplayResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region SetBIDCreate
        public ServiceResponse<SetBIDCreateResponse> SetBIDCreate(SetBIDCreate request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var bidcrerequest = request;
                    bidcrerequest.bidcreation.appidentifier = segment.Identifier();
                    bidcrerequest.bidcreation.appver = AppVersion;
                    bidcrerequest.bidcreation.lang = language.Code();
                    bidcrerequest.bidcreation.mobileosver = AppVersion;
                    bidcrerequest.bidcreation.sessionid = sessionId;
                    bidcrerequest.bidcreation.userid = userId;
                    bidcrerequest.bidcreation.vendorid = GetVendorId(segment);


                    var response = client.SetBIDCreate(bidcrerequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<SetBIDCreateResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<SetBIDCreateResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<SetBIDCreateResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<SetBIDCreateResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<SetBIDCreateResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region SetBIDUpdate
        public ServiceResponse<SetBIDUpdateResponse> SetBIDUpdate(SetBIDUpdate request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var biduprequest = request;
                    biduprequest.appidentifier = segment.Identifier();
                    biduprequest.appversion = AppVersion;
                    biduprequest.lang = language.Code();
                    biduprequest.mobileosversion = AppVersion;
                    biduprequest.sessionid = sessionId;
                    biduprequest.userid = userId;
                    biduprequest.vendorid = GetVendorId(segment);


                    var response = client.SetBIDUpdate(biduprequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<SetBIDUpdateResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<SetBIDUpdateResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<SetBIDUpdateResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<SetBIDUpdateResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<SetBIDUpdateResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region SetBIDWithdraw
        public ServiceResponse<SetBIDWithdrawResponse> SetBIDWithdraw(SetBIDWithdraw request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var bidwithrequest = request;
                    bidwithrequest.appidentifier = segment.Identifier();
                    bidwithrequest.appversion = AppVersion;
                    bidwithrequest.lang = language.Code();
                    bidwithrequest.mobileosversion = AppVersion;
                    bidwithrequest.sessionid = sessionId;
                    bidwithrequest.userid = userId;
                    bidwithrequest.vendorid = GetVendorId(segment);


                    var response = client.SetBIDWithdraw(bidwithrequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<SetBIDWithdrawResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<SetBIDWithdrawResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<SetBIDWithdrawResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<SetBIDWithdrawResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<SetBIDWithdrawResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region GetBIDDownload
        public ServiceResponse<GetBIDDownloadResponse> GetBIDDownload(GetBIDDownload request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var biddownrequest = request;
                    biddownrequest.appidentifier = segment.Identifier();
                    biddownrequest.appversion = AppVersion;
                    biddownrequest.lang = language.Code();
                    biddownrequest.mobileosversion = AppVersion;
                    biddownrequest.sessionid = sessionId;
                    biddownrequest.userid = userId;
                    biddownrequest.vendorid = GetVendorId(segment);


                    var response = client.GetBIDDownload(biddownrequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetBIDDownloadResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetBIDDownloadResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetBIDDownloadResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetBIDDownloadResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetBIDDownloadResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region GetBOQDownload
        public ServiceResponse<GetBOQDownloadResponse> GetBOQDownload(GetBOQDownload request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var boqdownrequest = request;
                    boqdownrequest.appidentifier = segment.Identifier();
                    boqdownrequest.appversion = AppVersion;
                    boqdownrequest.lang = language.Code();
                    boqdownrequest.mobileosversion = AppVersion;
                    boqdownrequest.sessionid = sessionId;
                    boqdownrequest.userid = userId;
                    boqdownrequest.vendorid = GetVendorId(segment);


                    var response = client.GetBOQDownload(boqdownrequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetBOQDownloadResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetBOQDownloadResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetBOQDownloadResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetBOQDownloadResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetBOQDownloadResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region GetBIDMainDownload
        public ServiceResponse<GetBIDMainDownloadResponse> GetBIDMainDownload(GetBIDMainDownload request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var bidmaindownrequest = request;
                    bidmaindownrequest.appidentifier = segment.Identifier();
                    bidmaindownrequest.appversion = AppVersion;
                    bidmaindownrequest.lang = language.Code();
                    bidmaindownrequest.mobileosversion = AppVersion;
                    bidmaindownrequest.sessionid = sessionId;
                    bidmaindownrequest.userid = userId;
                    bidmaindownrequest.vendorid = GetVendorId(segment);


                    var response = client.GetBIDMainDownload(bidmaindownrequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetBIDMainDownloadResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetBIDMainDownloadResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetBIDMainDownloadResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetBIDMainDownloadResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetBIDMainDownloadResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion


        //Sales order
        #region GetScrapOrders
        public ServiceResponse<GetScrapOrdersResponse> GetScrapOrders(GetScrapOrders request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var salesordrequest = request;
                    salesordrequest.appidentifier = segment.Identifier();
                    salesordrequest.appversion = AppVersion;
                    salesordrequest.lang = language.Code();
                    salesordrequest.mobileosversion = AppVersion;
                    salesordrequest.sessionid = sessionId;
                    salesordrequest.userid = userId;
                    salesordrequest.vendorid = GetVendorId(segment);


                    var response = client.GetScrapOrders(salesordrequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetScrapOrdersResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetScrapOrdersResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetScrapOrdersResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetScrapOrdersResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetScrapOrdersResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region GetSalesOrderDownload
        public ServiceResponse<GetSalesOrderDownloadResponse> GetSalesOrderDownload(GetSalesOrderDownload request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var salesdownrequest = request;
                    salesdownrequest.appidentifier = segment.Identifier();
                    salesdownrequest.appversion = AppVersion;
                    salesdownrequest.lang = language.Code();
                    salesdownrequest.mobileosversion = AppVersion;
                    salesdownrequest.sessionid = sessionId;
                    salesdownrequest.userid = userId;
                    salesdownrequest.vendorid = GetVendorId(segment);


                    var response = client.GetSalesOrderDownload(salesdownrequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetSalesOrderDownloadResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetSalesOrderDownloadResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetSalesOrderDownloadResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetSalesOrderDownloadResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetSalesOrderDownloadResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region GetEMDList
        public ServiceResponse<GetEMDListResponse> GetEMDList(GetEMDList request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var emdlistrequest = request;
                    emdlistrequest.appidentifier = segment.Identifier();
                    emdlistrequest.appversion = AppVersion;
                    emdlistrequest.lang = language.Code();
                    emdlistrequest.mobileosversion = AppVersion;
                    emdlistrequest.sessionid = sessionId;
                    emdlistrequest.userid = userId;
                    emdlistrequest.vendorid = GetVendorId(segment);


                    var response = client.GetEMDList(emdlistrequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetEMDListResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetEMDListResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetEMDListResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetEMDListResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetEMDListResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region GetScrapsalesTenderResultList
        public ServiceResponse<GetScrapsalesTenderResultListResponse> GetScrapsalesTenderResultList(GetScrapsalesTenderResultList request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);

                    var response = client.GetScrapsalesTenderResultList(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetScrapsalesTenderResultListResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetScrapsalesTenderResultListResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetScrapsalesTenderResultListResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetScrapsalesTenderResultListResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetScrapsalesTenderResultListResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region GetScrapOrderPayments
        public ServiceResponse<GetScrapOrderPaymentsResponse> GetScrapOrderPayments(GetScrapOrderPayments request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var salesordpayrequest = request;
                    salesordpayrequest.appidentifier = segment.Identifier();
                    salesordpayrequest.appversion = AppVersion;
                    salesordpayrequest.lang = language.Code();
                    salesordpayrequest.mobileosversion = AppVersion;
                    salesordpayrequest.sessionid = sessionId;
                    salesordpayrequest.userid = userId;
                    salesordpayrequest.vendorid = GetVendorId(segment);


                    var response = client.GetScrapOrderPayments(salesordpayrequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetScrapOrderPaymentsResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetScrapOrderPaymentsResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetScrapOrderPaymentsResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetScrapOrderPaymentsResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetScrapOrderPaymentsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region GetScrapAccountDocumentDownload
        public ServiceResponse<GetScrapAccountDocumentDownloadResponse> GetScrapAccountDocumentDownload(GetScrapAccountDocumentDownload request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var salesdocdownrequest = request;
                    salesdocdownrequest.appidentifier = segment.Identifier();
                    salesdocdownrequest.appversion = AppVersion;
                    salesdocdownrequest.lang = language.Code();
                    salesdocdownrequest.mobileosversion = AppVersion;
                    salesdocdownrequest.sessionid = sessionId;
                    salesdocdownrequest.userid = userId;
                    salesdocdownrequest.vendorid = GetVendorId(segment);


                    var response = client.GetScrapAccountDocumentDownload(salesdocdownrequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetScrapAccountDocumentDownloadResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetScrapAccountDocumentDownloadResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetScrapAccountDocumentDownloadResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetScrapAccountDocumentDownloadResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetScrapAccountDocumentDownloadResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region GetScrapAccountDocumentDownload
        public ServiceResponse<GetTenderReceiptResponse> GetTenderReceipt(GetTenderReceipt request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var tenderreceiptrequest = request;
                    tenderreceiptrequest.appidentifier = segment.Identifier();
                    tenderreceiptrequest.appversion = AppVersion;
                    tenderreceiptrequest.lang = language.Code();
                    tenderreceiptrequest.mobileosversion = AppVersion;
                    tenderreceiptrequest.sessionid = sessionId;
                    tenderreceiptrequest.userid = userId;
                    tenderreceiptrequest.vendorid = GetVendorId(segment);


                    var response = client.GetTenderReceipt(tenderreceiptrequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetTenderReceiptResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetTenderReceiptResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetTenderReceiptResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetTenderReceiptResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetTenderReceiptResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        // Profile
        #region SetProfileUpdate
        public ServiceResponse<SetProfileUpdateResponse> SetProfileUpdate(SetProfileUpdate request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var _profilerequest = request;
                    _profilerequest.profilerequest.appidentifier = segment.Identifier();
                    _profilerequest.profilerequest.appversion = AppVersion;
                    _profilerequest.profilerequest.lang = language.Code();
                    _profilerequest.profilerequest.mobileosversion = AppVersion;
                    _profilerequest.profilerequest.sessionid = sessionId;
                    _profilerequest.profilerequest.userid = userId;
                    _profilerequest.profilerequest.vendorid = GetVendorId(segment);


                    var response = client.SetProfileUpdate(_profilerequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<SetProfileUpdateResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<SetProfileUpdateResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<SetProfileUpdateResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<SetProfileUpdateResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<SetProfileUpdateResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion


        #region GetSalesOrderDownloadOTP

        public ServiceResponse<GetSalesOrderDownloadOTPResponse> GetSalesOrderDownloadOTP(GetSalesOrderDownloadOTP request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var _getotprequest = request;
                    _getotprequest.appidentifier = segment.Identifier();
                    _getotprequest.appversion = AppVersion;
                    _getotprequest.lang = language.Code();
                    _getotprequest.mobileosversion = AppVersion;
                    _getotprequest.vendorid = GetVendorId(segment);


                    var response = client.GetSalesOrderDownloadOTP(_getotprequest);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetSalesOrderDownloadOTPResponse>(response);

                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        return new ServiceResponse<GetSalesOrderDownloadOTPResponse>(response, true, response.@return.description);

                    }
                    return new ServiceResponse<GetSalesOrderDownloadOTPResponse>(response, false, response.@return.description);

                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetSalesOrderDownloadOTPResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetSalesOrderDownloadOTPResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #endregion
    }
}
