using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.InternshipSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration
{
    public interface IinternshipServiceClient
    {
		ServiceResponse<SetInternshipRegistrationResponse> SetInternshipRegistration(SetInternshipRegistration request);
		ServiceResponse<GetInternshipHelpValuesResponse> GetHelpValues(GetInternshipHelpValues request, SupportedLanguage language = SupportedLanguage.English);



	}
}
