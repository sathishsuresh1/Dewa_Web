using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Impl.WebsiteSurveySvc;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration
{
    public interface IWebsiteSurveyServiceClient
    {
        #region Save Website Surrvey
        ServiceResponse<WebsiteSurveyResponse> SaveWebsiteSurvey(QuestionAnswers SurveyData);
        #endregion
    }
}
