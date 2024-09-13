// <copyright file="IDewaStoreClient.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration
{
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.DewaStoreSvc;

    /// <summary>
    /// Defines the <see cref="IDewaStoreClient" />.
    /// </summary>
    public interface IDewaStoreClient
    {
        /// <summary>
        /// The GetAllWebOffers.
        /// </summary>
        /// <param name="IsHotOffer">The IsHotOffer<see cref="int"/>.</param>
        /// <param name="IsFeaturedOffer">The IsFeaturedOffer<see cref="int"/>.</param>
        /// <param name="IsNewOffer">The IsNewOffer<see cref="int"/>.</param>
        /// <param name="CompanyUno">The CompanyUno<see cref="string"/>.</param>
        /// <param name="CategoryUno">The CategoryUno<see cref="string"/>.</param>
        /// <param name="PageSize">The PageSize<see cref="int"/>.</param>
        /// <param name="PageNumber">The PageNumber<see cref="int"/>.</param>
        /// <param name="Condition">The Condition<see cref="int"/>.</param>
        /// <param name="ContractAccNumber">The ContractAccNumber<see cref="string"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <returns>The <see cref="ServiceResponse{OfferBaseResponse}"/>.</returns>
        ServiceResponse<OfferBaseResponse> GetAllWebOffers(int IsHotOffer, int IsFeaturedOffer, int IsNewOffer, string CompanyUno, string CategoryUno, int PageSize, int PageNumber, int Condition, string ContractAccNumber, SupportedLanguage language);
        ServiceResponse<OfferBaseResponse> GetAllGuestOffers(int IsHotOffer, int IsFeaturedOffer, int IsNewOffer, string CompanyUno, string CategoryUno, int PageSize, int PageNumber, int Condition, SupportedLanguage language);
    }
}
