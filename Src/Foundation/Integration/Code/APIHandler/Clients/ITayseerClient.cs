using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Requests.Tayseer;
using DEWAXP.Foundation.Integration.Responses.Tayseer;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface ITayseerClient
    {
        ServiceResponse<TayseerDetailsResponse> TayseerDetails(TayseerDetailsRequest request, string userId, string userSessionId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<TayseerListResponse> TayseerList(TayseerDetailsRequest request, string userId, string userSessionId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<TayseerCreateResponse> TayseerCreateReference(TayseerDetailsRequest request, string userId, string userSessionId, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
    }
}
