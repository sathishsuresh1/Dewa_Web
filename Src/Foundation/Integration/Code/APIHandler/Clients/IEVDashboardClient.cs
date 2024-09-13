// <copyright file="IEVDashboardClient.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
    using DEWAXP.Foundation.Integration.Responses;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="IEVDashboardClient" />.
    /// </summary>
    public interface IEVDashboardClient
    {
        /// <summary>
        /// The FetchActiveEVCards.
        /// </summary>
        /// <param name="request">The request<see cref="FetchEVCardsRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{List{Evcarddetail}}"/>.</returns>
        ServiceResponse<List<Evcarddetail>> FetchActiveEVCards(FetchEVCardsRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The UpdateEVCard.
        /// </summary>
        /// <param name="request">The request<see cref="UpdateEVCardRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{List{Evcarddetail}}"/>.</returns>
        ServiceResponse<List<Evcarddetail>> UpdateEVCard(UpdateEVCardRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetEVConsumptionDetails.
        /// </summary>
        /// <param name="request">The request<see cref="EVConsumptionRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{EVConsumptionResponse}"/>.</returns>
        ServiceResponse<EVConsumptionResponse> GetEVConsumptionDetails(EVConsumptionRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetEVTransactionalDetails.
        /// </summary>
        /// <param name="request">The request<see cref="EVConsumptionRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{EVTransactionalResponse}"/>.</returns>
        ServiceResponse<EVTransactionalResponse> GetEVTransactionalDetails(EVConsumptionRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetOutstandingBreakDown.
        /// </summary>
        /// <param name="request">The request<see cref="EVBillsRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{List{Outstandingbreakdown}}"/>.</returns>
        ServiceResponse<List<Outstandingbreakdown>> GetOutstandingBreakDown(EVBillsRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetEVSDPayment.
        /// </summary>
        /// <param name="request">The request<see cref="EVDeeplinkRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{EVDeepLinkResponse}"/>.</returns>
        ServiceResponse<EVDeepLinkResponse> GetEVSDPayment(EVDeeplinkRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
