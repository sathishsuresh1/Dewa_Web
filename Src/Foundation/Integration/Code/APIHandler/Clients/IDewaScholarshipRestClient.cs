using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.DewaScholarship;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.DewaScholarship;
using DEWAXP.Foundation.Integration.Enums;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
   public interface IDewaScholarshipRestClient
    {
        ServiceResponse<EmailVerificationResponse> EmailVerification(EmailVerificationRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
    }
}
