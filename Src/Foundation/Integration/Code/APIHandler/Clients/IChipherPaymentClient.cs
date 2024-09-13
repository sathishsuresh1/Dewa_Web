using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SecuredPayment;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.SecuredPayment;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IChipherPaymentClient
    {
        ServiceResponse<CipherPaymentDetailResponse> GenerateEpayToken(CipherPaymentDetailRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
    }
}
