using DEWAXP.Foundation.Integration.APIHandler.Models.Request.General;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.General;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IGeneralServicesClient
    {
        ServiceResponse<MaiDubaiResponse> DecryptURL(MaiDubaiRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
