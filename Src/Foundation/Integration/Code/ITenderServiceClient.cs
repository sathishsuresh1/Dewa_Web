using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.TenderSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration
{
    public interface ITenderServiceClient
    {
	    /// <summary>
	    /// Gets a list of all tenders (open and closed)
	    /// </summary>
	    /// <param name="language"></param>
	    /// <returns>A list of available tenders</returns>
	    ServiceResponse<GetOpenTenderList> GetOpenTenderList(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<GetTenderOpeningListResponse> GetTenderOpeningList(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<TenderItemResultResponse> GetTenderOpeningResult(string fid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
