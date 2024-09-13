// <copyright file="IGatePassServiceClient.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration
{
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.GatePassSvc;
    using DEWAXP.Foundation.Integration.Responses;

    /// <summary>
    /// Defines the <see cref="IGatePassServiceClient" />
    /// </summary>
    public interface IGatePassServiceClient
    {
        /// <summary>
        /// The GetGPSupplierDetails
        /// </summary>
        /// <param name="userinput">The userinput<see cref="userDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{supplierDetailsOutput}"/></returns>
        ServiceResponse<supplierDetailsOutput> GetGPSupplierDetails(userDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The CreateGPUser
        /// </summary>
        /// <param name="userinput">The userinput<see cref="createDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        ServiceResponse<userDetailsOutput> CreateGPUser(createDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The UserLogin
        /// </summary>
        /// <param name="logininput">The logininput<see cref="logInOutDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{loginDetailsOutput}"/></returns>
        ServiceResponse<loginDetailsOutput> UserLogin(logInOutDataInput logininput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The UserDetails
        /// </summary>
        /// <param name="logininput">The logininput<see cref="userDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        ServiceResponse<userDetailsOutput> UserDetails(userDataInput logininput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The UserLogout
        /// </summary>
        /// <param name="logininput">The logininput<see cref="logInOutDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{logoutDetailsOutput}"/></returns>
        ServiceResponse<logoutDetailsOutput> UserLogout(logInOutDataInput logininput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The UserActivation
        /// </summary>
        /// <param name="userinput">The userinput<see cref="userDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        ServiceResponse<userDetailsOutput> UserActivation(userDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The ForgotUserid
        /// </summary>
        /// <param name="userinput">The userinput<see cref="credentialDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        ServiceResponse<userDetailsOutput> ForgotUserid(credentialDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The ForgotPassword
        /// </summary>
        /// <param name="userinput">The userinput<see cref="credentialDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        ServiceResponse<userDetailsOutput> ForgotPassword(credentialDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The SetNewPassword
        /// </summary>
        /// <param name="userinput">The userinput<see cref="credentialDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        ServiceResponse<userDetailsOutput> SetNewPassword(credentialDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The ChangePassword
        /// </summary>
        /// <param name="userinput">The userinput<see cref="credentialDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        ServiceResponse<userDetailsOutput> ChangePassword(credentialDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The ProjectList
        /// </summary>
        /// <param name="userinput">The userinput<see cref="userDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{poDetailsOutput}"/></returns>
        ServiceResponse<poDetailsOutput> ProjectList(userDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
