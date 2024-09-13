using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    public interface IEVCardApiHandler
    {
        ServiceResponse<Models.Response.EvPlateDetailsResponse> GetEvPlateDetails(Models.ApiBaseRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        //ServiceResponse<Models.Response.SetNewEVGreenCardV3Response> SetNewEVGreenCardV3(Models.Request.SetNewEVGreenCardV3Request request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
