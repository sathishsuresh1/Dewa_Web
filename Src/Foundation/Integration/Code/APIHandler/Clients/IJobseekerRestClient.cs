using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Common;
using DEWAXP.Foundation.Integration.JobSeekerSvc;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IJobseekerRestClient
    {
        ServiceResponse<LoginResponse> LoginUser(LoginRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<OTPResponse> VerifyOtp(OTPRequest request, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<CommonResponse> UnlockAccount(UnlockAccountRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<CommonResponse> ForgotUseridPwd(ForgotPasswordRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<ForgotUserNameResponse> ForgotUserName(ForgotUserNameRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
