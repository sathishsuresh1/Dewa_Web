using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IMoveOutClient
    {
        ServiceResponse<Models.Response.MoveOut.MoveOutResponse> SetMoveOutRequestV2(Models.Request.MoveOut.MoveoutRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
    }
}
