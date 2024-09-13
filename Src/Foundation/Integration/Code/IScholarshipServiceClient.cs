using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.ScholarshipService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration
{
    public interface IScholarshipServiceClient
    {
        ServiceResponse<string> NewRegistration(
           string programID, string fName, string mName, string lName,
           string mobileNo, string username, string password, string password1,
           string email, string email1, string emiratesId, bool register,
           SupportedLanguage language, RequestSegment segment);
        ServiceResponse<candidateDetails> UpdateCandidateDetails(candidateDetails canDetails, string stage, string credentials, string username,
           SupportedLanguage language, RequestSegment segment);

        ServiceResponse<LoginScholarshipResponse> SignIn(string userId, string password, SupportedLanguage language, RequestSegment segment, string credential);
        ServiceResponse ForgotPassword(string userId, string email, SupportedLanguage language, RequestSegment segment);
        ServiceResponse RequestUsername(string email, SupportedLanguage language, RequestSegment segment);
        ServiceResponse SetNewPassword(string username, string currentPassword, string newPassword,
          string credentials, SupportedLanguage language, RequestSegment segment);

        ServiceResponse<scholarshipHelpValues> GetHelpValuesInEnglish();
        ServiceResponse<scholarshipHelpValues> GetHelpValuesInArabic();
    }
}
