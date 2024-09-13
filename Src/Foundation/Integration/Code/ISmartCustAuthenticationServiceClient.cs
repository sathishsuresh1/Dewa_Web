using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.SmartCustAuthenticationSvc;

namespace DEWAXP.Foundation.Integration
{
    public interface ISmartCustAuthenticationServiceClient
    {
       ServiceResponse<SmartCustAuthenticationResponse> GetLoginSessionCustomerAuthentication(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
