// <copyright file="ISmartVendorClient.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Impl.SmartVendor
{
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Requests.SmartVendor;
    using DEWAXP.Foundation.Integration.Requests.SmartVendor.WorkPermit;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.SmartVendor.WorkPermit;

    /// <summary>
    /// Defines the <see cref="ISmartVendorClient" />.
    /// </summary>
    public interface ISmartVendorClient
    {
        /// <summary>
        /// The SubmitWorkPermitPass.
        /// </summary>
        /// <param name="model">The model<see cref="GroupPemitPassRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{GroupPassPemitResponse}"/>.</returns>
        ServiceResponse<GroupPassPemitResponse> SubmitWorkPermitPass(GroupPemitPassRequest model, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetICADetails.
        /// </summary>
        /// <param name="model">The model<see cref="UserDetailsRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{UserDetailsResponse}"/>.</returns>
        ServiceResponse<UserDetailsResponse> GetICADetails(UserDetailsRequest model, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetTradeLicenseDetails.
        /// </summary>
        /// <param name="model">The model<see cref="TradeLicenseRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{TradeLicenseResponse}"/>.</returns>
        ServiceResponse<TradeLicenseResponse> GetTradeLicenseDetails(TradeLicenseRequest model, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
