using DEWAXP.Foundation.Integration.APIHandler.Models.Request.PremiseNumberSearch;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.PremiseNumberSearch;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IPremiseNumberSearchClient
    {
         ServiceResponse<PremiseNumberSearchResponse> PremiseNumberSearch(PremiseNumberSearchRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
