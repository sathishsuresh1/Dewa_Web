using ResponseModels = DEWAXP.Foundation.Integration.APIHandler.Models.Response.SelfEnergyAssessmentSurvey;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using RequestModels = DEWAXP.Foundation.Integration.APIHandler.Models.Request.SelfEnergyAssessmentSurvey;

using System.Collections.Generic;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface ISelfEnergyAssessmentSurveyClient
    {
        ServiceResponse<ResponseModels.QuestionsAndAnswersResponse> GetQuestionsAndAnswers(string userSessionId, string userId, string contractAccount, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<ResponseModels.SavedAnsersResponse> GetSavedAnswers(string userSessionId, string userId, string contractAccount, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<ResponseModels.SaveSurveyResponse> SaveSurveyAnswers(string userSessionId, string userId, string contractAccount, List<RequestModels.Answerlist> answers, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<ResponseModels.SubmitSurveyResponse> SubmitSurveyAnswers(string userSessionId, string userId, string contractAccount, List<RequestModels.Answerlist> answers, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<ResponseModels.SubmittedSurveysResponse> GetSubmittedSurveys(string userSessionId, string userId, string contractAccount, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<string> DownloadSurveyReport(string userSessionId, string userId, string contractAccount,string version, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<string> EmailSurveyReport(string userSessionId, string userId, string contractAccount, string version, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
    }
}
