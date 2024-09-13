// <copyright file="PaymentHelper.cs">
// Copyright (c) 2023
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Helpers
{
    using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Payment;
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Impl.SmartCustomerV3Svc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Defines the <see cref="PaymentHelper" />.
    /// </summary>
    public static class PaymentHelper
    {
        /// <summary>
        /// Defines the _smartCustomerClient.
        /// </summary>
        private static readonly ISmartCustomerClient _smartCustomerClient = DependencyResolver.Current.GetService<ISmartCustomerClient>();

        /// <summary>
        /// The UAEPGSList.
        /// </summary>
        /// <param name="lang">The lang<see cref="string"/>.</param>
        /// <param name="segment">The segment<see cref="string"/>.</param>
        /// <returns>The <see cref="List{Uaepgsdetail}"/>.</returns>
        public static List<Uaepgsdetail> UAEPGSList(string lang, string segment)
        {
            SupportedLanguage supportedLanguage = SupportedLanguage.Arabic;
            if (!string.IsNullOrWhiteSpace(lang) && lang.ToLower().Equals("en"))
            {
                supportedLanguage = SupportedLanguage.English;
            }
            Integration.Responses.ServiceResponse<UAEPGSResponse> response = _smartCustomerClient.UAEPGSList(supportedLanguage, Extensions.HttpRequestExtensions.DetermineSegment(segment));
            if (response != null && response.Succeeded && response.Payload != null && response.Payload.Uaepgsdetails != null && response.Payload.Uaepgsdetails.Count > 0)
            {
                return response.Payload.Uaepgsdetails;
            }
            return null;
        }
    }
}
