using DEWAXP.Foundation.Integration.APIHandler.Models.Request.UAEPassService;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.UAEPassService;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IUAEPassServiceClient
    {
        ServiceResponse<UAEPassDubaiIdLoginResponse> UAEPASSDubaiIdLogin(UAEPassDubaiIdLoginRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<dynamic> UAEPASSCustomerAuthentication(UAEPassDubaiIdLoginRequest input);
        ServiceResponse<string> UAEPASSCustomerData(string access_token, string token_type);
    }
}
