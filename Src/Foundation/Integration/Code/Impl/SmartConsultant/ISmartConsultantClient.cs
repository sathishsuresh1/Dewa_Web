// <copyright file="ISmartConsultantClient.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration
{
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Requests;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.SmartConsultant;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="ISmartConsultantClient" />.
    /// </summary>
    public interface ISmartConsultantClient
    {
        /// <summary>
        /// The DisplayPVCCTraining.
        /// </summary>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <returns>The <see cref="ServiceResponse{List{TrainingDetail}}"/>.</returns>
        ServiceResponse<List<TrainingDetail>> DisplayPVCCTraining(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<UpdateConsultantTrainingResponse> BookingPVCCTraining(UpdateConsultantTrainingRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<UpdateConsultantTrainingAttachResponse> BookingPVCCTrainingAttach(UpdateConsultantTrainingAttachRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<TrainingBookingDetailsResponse> TrainingBookingDetails(TrainingBookingDetailsRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<PVCCCertificateDetailsResponse> PVCCCertificateDetails(PVCCCertificateDetailsRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<PVCCEmiratesidVerifyResponse> PVCCEmiratesidVerify(PVCCEmiratesidVerifyRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
