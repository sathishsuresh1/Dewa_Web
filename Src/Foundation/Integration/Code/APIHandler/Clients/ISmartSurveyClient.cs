using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartSurvey;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.SmartSurvey;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface ISmartSurveyClient
    {
        /// <summary>
        /// The SurveyData.
        /// </summary>
        /// <param name="request">The request<see cref="string"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{string}"/>.</returns>
        ServiceResponse<Surveydataoutput> SurveyData(SurveyDataInput request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<SurveyOTPOutputModel> SurveyOtp(SurveyOTPInput request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

    }
}
