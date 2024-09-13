using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration
{
    public interface IVerificationcodeServiceClient
    {
       ServiceResponse<QRCodeResponse> getQRCodeVerified(string certificatetype, string referencenumber, string pinnumber, SupportedLanguage language=SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
