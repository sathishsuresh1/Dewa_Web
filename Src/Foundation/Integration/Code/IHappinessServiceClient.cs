using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.HappinessSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration
{
    public interface IHappinessServiceClient
    {
        ServiceResponse<HappinessSurveyResponse> GetHappinessSurvey(string surveyType);
        ServiceResponse SubmitHappinessSurvey(AnswersResponse[] arrAnswers, string referenceNo, string surveyType);
    }
}
