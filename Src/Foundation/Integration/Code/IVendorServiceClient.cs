// <copyright file="IVendorServiceClient.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration
{
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.VendorSvc;
    using DEWAXP.Foundation.Integration.SmartVendorSvc;

    /// <summary>
    /// Defines the <see cref="IVendorServiceClient" />.
    /// </summary>
    public interface IVendorServiceClient
    {
        /// <summary>
        /// Gets a list of all open Rfx's.
        /// </summary>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>A list of available tenders.</returns>
        ServiceResponse<SRM_OpenInquiries> GetOpenRFXInquiries(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Gets a file (pdf) as a byte array for the corresponding rFx.
        /// </summary>
        /// <param name="rfxId">retrieved from the Rfx list.</param>
        /// <param name="language">.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>.</returns>
        ServiceResponse<byte[]> GetExportRfx(string rfxId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Get Tender Result List.
        /// </summary>
        /// <param name="language">.</param>
        /// <param name="segment">.</param>
        /// <returns>.</returns>
        ServiceResponse<GetTenderResultListDataResponse> GetTenderResultList(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Get Tender Result Display.
        /// </summary>
        /// <param name="tenderNumber">.</param>
        /// <param name="language">.</param>
        /// <param name="segment">.</param>
        /// <returns>.</returns>
        ServiceResponse<GetTenderResultDisplayDataResponse> GetTenderResultDisplay(string tenderNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Get Open Tender List.
        /// </summary>
        /// <param name="language">.</param>
        /// <param name="segment">.</param>
        /// <returns>.</returns>
        ServiceResponse<GetOpenTenderListDataResponse> GetOpenTenderList(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Get Advertised Tender Document.
        /// </summary>
        /// <param name="tenderNumber">.</param>
        /// <param name="language">.</param>
        /// <param name="segment">.</param>
        /// <returns>.</returns>
        ServiceResponse<GetFileResponse> GetTenderAdvertisment(string tenderNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetWorkPermitPass.
        /// </summary>
        /// <param name="input">The input<see cref="GetWorkPermitPass"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{workPermitResponseDetails}"/>.</returns>
        ServiceResponse<workPermitResponseDetails> GetWorkPermitPass(GetWorkPermitPass input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetWorkPermitPass.
        /// </summary>
        /// <param name="input">The input<see cref="GetCountryList"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{countryListResponse}"/>.</returns>
        ServiceResponse<countryListResponse> GetCountryList(GetCountryList input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetPOList.
        /// </summary>
        /// <param name="input">The input<see cref="GetPOList"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{POListResponse}"/>.</returns>
        ServiceResponse<POListResponse> GetPOList(GetPOList input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetWorkPermitSubContract.
        /// </summary>
        /// <param name="input">The input<see cref="GetWorkPermitSubContract"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{subContractorResponse}"/>.</returns>
        ServiceResponse<subContractorResponse> GetWorkPermitSubContract(GetWorkPermitSubContract input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
