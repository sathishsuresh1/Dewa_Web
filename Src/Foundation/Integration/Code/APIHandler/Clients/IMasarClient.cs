using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartCommunication;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Common;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.SmartCommunication;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    using @MasarRequest = Models.Request.Masar;
    using @MasarResponse = Models.Response.Masar;
    public interface IMasarClient
    {
        ServiceResponse<@MasarResponse.MasarDropDownBaseResponse> GetMasarDropDownData(@MasarRequest.MasarDropdownBaseRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarOTPResponse> MasarSendOtp(MasarOTPRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);

        ServiceResponse<MasarUserRegistrationResponse> ScrapRegistration(MasarUserRegistrationRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarAttachmentResponse> AddMasarAttachments(MasarAttachmentRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarLoginResponse> MasarLogin(MasarLoginRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarGetMaskedEmailNMobileResponse> MasarGetMaskedEmailNPhone(MasarGetMaskedEmailNMobileRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userSessionId"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<MasarBankResponse> GetBankList(string userId, string userSessionId, string requestNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<CommonResponse> UpdatePassword(MasarChangePasswordRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarForgetPasswordResponse> MasarForgotPassword(MasarForgetPasswordRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);

        ServiceResponse<MasarProfileFetchResponse> MasarProfileFetch(MasarRequest.MasarProfileFetchRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarDecryptGUIDResponse> MasarCreateUserDecryptGUID(MasarDecryptGUIDRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);

        ServiceResponse<MasarCreateUserCredentialResponse> MasarCreateUserCredential(MasarCreateUserCredentialRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<AddBankResponse> AddBank(MasarAddBankRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarForgotUserNameResponse> MasarForgotUserName(MasarForgotUserNameRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarTrackApplicationResponse> MasarTrackApplication(MasarTrackApplicationRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarTrackMiscellaneousResponse> MasarMiscellaneousTrackApplication(MasarTrackMiscellaneousRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarFetchICAResponse> MasarFetchICA(MasarFetchICARequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarFetchDEDResponse> MasarFetchDED(MasarFetchDEDRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<ReadOtpNbResponse> MasarReadOtpNB(ReadOtpNbRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<MasarCancelBankResponse> CancelBankRequest(MasarCancelBankRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<MasarDisplayBankResponse> DisplayBankRequest(MasarDisplayBankRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<MasarTrackInputResponse> FetchTrackApplicationData(MasarTrackInputRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }


}
