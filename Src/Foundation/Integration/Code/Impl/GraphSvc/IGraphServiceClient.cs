using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.GraphSvc;

namespace DEWAXP.Foundation.Integration
{
    public interface IGraphServiceClient
    {
        ServiceResponse<RootObject> GetGraph(string contractAccountNumber, string date, string month, string year, string usagetype,string userid,string session,SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}