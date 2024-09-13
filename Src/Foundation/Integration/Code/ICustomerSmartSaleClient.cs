using System;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.CustomerSmartSalesSvc;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Requests;

namespace DEWAXP.Foundation.Integration
{
    public interface ICustomerSmartSaleClient
    {
        ServiceResponse<GetLoginSessionScrapCustomerResponse> GetLoginSessionScrapCustomer(GetLoginSessionScrapCustomer request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse GetScrapCustomerForgotUserID(GetScrapCustomerForgotUserID request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse SetScrapCustomerPasswordChange(SetScrapCustomerPasswordChange request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse SetScrapCustomerNewPassword(SetScrapCustomerPasswordValidate request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<SetScrapRegistrationResponse> SetScrapRegistration(SetScrapRegistration request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetRegistrationScrapCustomerDetailsResponse> GetRegistrationScrapCustomerDetails(GetRegistrationScrapCustomerDetails request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<SetScrapCustomerAccountRegistrationResponse> SetScrapCustomerAccountRegistration(SetScrapCustomerAccountRegistration request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<SendVerifyScrapRegistrationCodeResponse> SendVerifyScrapRegistrationCode(SendVerifyScrapRegistrationCode request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse GetUserIDCheck(GetUserIDCheck request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        //GetLoginOpenTenderList
        ServiceResponse<GetLoginOpenTenderListResponse> GetLoginOpenTenderList(GetLoginOpenTenderList request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        //GetTenderDocumentDownload
        ServiceResponse<GetTenderDocumentDownloadResponse> GetTenderDocumentDownload(GetTenderDocumentDownload request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        //GetTenderReceipt
        ServiceResponse<GetTenderReceiptResponse> GetTenderReceipt(GetTenderReceipt request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        //GetTenderReferenceNumber
        ServiceResponse<GetTenderReferenceNumberResponse> GetTenderReferenceNumber(GetTenderReferenceNumber request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetCountryHelpValuesResponse> GetCountryHelpValues(GetCountryHelpValues request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<GetTenderAdvertisementResponse> GetTenderAdvertisement(GetTenderAdvertisement request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetSSTenderPayStatusResponse> GetSSTenderPayStatus(GetSSTenderPayStatus request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetOpenTenderListResponse> GetOpenTenderList(GetOpenTenderList request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetOpenTenderListRequestedResponse> GetOpenTenderListRequested(GetOpenTenderListRequested request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetTenderListPurchasedHistoryResponse> GetTenderListPurchasedHistory(GetTenderListPurchasedHistory request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse SetScrapCustomerPasswordReset(SetScrapCustomerPasswordReset request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        #region Phase-2 
        ServiceResponse<GetBOQDisplayResponse> GetBOQDisplay(GetBOQDisplay request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<SetBIDCreateResponse> SetBIDCreate(SetBIDCreate request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<SetBIDUpdateResponse> SetBIDUpdate(SetBIDUpdate request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<SetBIDWithdrawResponse> SetBIDWithdraw(SetBIDWithdraw request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetBIDDownloadResponse> GetBIDDownload(GetBIDDownload request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetBOQDownloadResponse> GetBOQDownload(GetBOQDownload request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetBIDMainDownloadResponse> GetBIDMainDownload(GetBIDMainDownload request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        // Sales order
        ServiceResponse<GetScrapOrdersResponse> GetScrapOrders(GetScrapOrders request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetSalesOrderDownloadResponse> GetSalesOrderDownload(GetSalesOrderDownload request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetEMDListResponse> GetEMDList(GetEMDList request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetScrapsalesTenderResultListResponse> GetScrapsalesTenderResultList(GetScrapsalesTenderResultList request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetScrapOrderPaymentsResponse> GetScrapOrderPayments(GetScrapOrderPayments request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetScrapAccountDocumentDownloadResponse> GetScrapAccountDocumentDownload(GetScrapAccountDocumentDownload request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetTenderReceiptResponse> GetTenderReceipt(GetTenderReceipt request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        // Profile
        ServiceResponse<SetProfileUpdateResponse> SetProfileUpdate(SetProfileUpdate request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<GetSalesOrderDownloadOTPResponse> GetSalesOrderDownloadOTP(GetSalesOrderDownloadOTP request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        #endregion
    }
}
