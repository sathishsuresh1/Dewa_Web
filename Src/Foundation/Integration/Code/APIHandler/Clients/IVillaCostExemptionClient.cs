using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.VillaCostExemption;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Requests.VillaCostExemption;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IVillaCostExemptionClient
    {
        ServiceResponse<DashboardResponse> GetDashboardResources(string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<ApplicationResponse> GetApplicationResources(NewApplicationRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<ApplicationResponse> SaveApplication(NewApplicationRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<ApplicationResponse> RetrieveApplication(NewApplicationRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<DownloadFileResponse> DownloadFile(DownloadFileRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<UploadFileResponse> UploadFile(UploadFileRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
         ServiceResponse<ICADetailsResponse> GetICADetails(ICADetailsRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<CountryListResponse> GetCountryList(CountryListRequest request, string userSessionId, string userId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);

    }
}
