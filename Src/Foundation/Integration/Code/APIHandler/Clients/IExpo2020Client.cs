using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Expo2020;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Expo2020;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IExpo2020Client
    {
         ServiceResponse<Expo2020Response> FeedbackExpo(Expo2020Request request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<Expo2020MasterDataResponse> MasterDataExpo(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
