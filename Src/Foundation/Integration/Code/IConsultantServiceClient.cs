using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.ConsultantSvc;
using DEWAXP.Foundation.Integration.SmartConsultantSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration
{
    public interface IConsultantServiceClient
    {
        ServiceResponse<GetLoginSessionConsultantResponse> GetLoginSessionOwnerConsultant(GetLoginSessionConsultant request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<ConnectionCalculatorResponse> GetConnectionCalCharges(string existingLoad, string proposeLoad, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<GetSurveyQuestionsResponse> GetSurveyQuestions(GetSurveyQuestions request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<PostSurveyAnswersResponse> PostSurveyAnswers(PostSurveyAnswers request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<ValidateSurveyResponse> ValidateSurvey(ValidateSurvey request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<TrackOwnerResponse> GetTrackOwner(TrackOwner request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<TrackOwnerOrderResponse> GetTrackOwnerOrder(TrackOwnerOrder request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<TrackOwnerStatusResponse> GetTrackOwnerStatus(TrackOwnerStatus request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
