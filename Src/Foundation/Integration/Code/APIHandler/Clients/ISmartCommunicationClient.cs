using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartCommunication;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.SmartCommunication;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface ISmartCommunicationClient
    {
        ServiceResponse<SmartCommunicationResponse> SmartCommunicationSubmit(SmartCommunicationRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<SmartCommunicationVerifyOtpResponse> VerifyMobileOtp(SmartCommunicationVerifyOtpRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<SessionLoginResponse> SessionLogin(SessionLoginRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<CustomerUpdateOtpResponse> CustomerVerifyOtp(SmartCommunicationVerifyOtpRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
    }
}
