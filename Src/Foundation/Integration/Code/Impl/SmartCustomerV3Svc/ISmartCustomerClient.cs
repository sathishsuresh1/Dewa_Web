using DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Common;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Requests;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVSmartCharger;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.SmartConsultant;
using DEWAXP.Foundation.Integration.Responses.SmartCustomer;
using DEWAXP.Foundation.Integration.Responses.Emirates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Payment;
//using AccountDetails = DEWAXP.Foundation.Integration.Responses.SmartCustomer.AccountDetails;

namespace DEWAXP.Foundation.Integration.Impl.SmartCustomerV3Svc
{
    public interface ISmartCustomerClient
    {
        //ServiceResponse<slabTarrifResponse> GetSlabCaps(slabTarrifRequest request, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<AccountDetails[]> GetCAList(string userId, string sessionId, string checkMoveOut, string ServiceFlag, bool includeInactive = false, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<SmartMeterResponse> GetSmartMeterDetails(SmartMeterRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<OneTimeChargeResponse> EVSmartCharger(OneTimeChargeRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<BillHistoryResponse> GetBillPaymentHistory(BillHistoryRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<InfrastructureNocResponse> GetInfrastructureNocList(InfrastructureNocRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<InfrastructureNocResponse> GetInfrastructureNocDetails(InfrastructureNocRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<InfrastructureNocWorkTypeResponse> GeWorkTypes(InfrastructureNocRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<InfrastructureNocSubmitReponse> SubmitNewNocRequest(InfraNocSubmitRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<InfrastructureNocAttachmentResponse> DownloadFile(InfrastructureNocRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<InfrastructureNocStatusResponse> GetStatusDetails(InfrastructureNocRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<InfranocActiveAccountResponse> InfranocActiveAccount(InfranocActiveAccountRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<EmiratesIDIntegrationResponse> GetEmiratesIDdetails(EmiratesIDIntegrationRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<CommonResponse> ForgotUseridPwd(ForgotPasswordRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<CommonResponse> UnlockAccount(UnlockAccountRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<LoginResponse> LoginUser(LoginRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<UAEPGSResponse> UAEPGSList(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
