// <copyright file="FetchEVCardsRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="FetchEVCardsRequest" />.
    /// </summary>
    public class FetchEVCardsRequest
    {
        /// <summary>
        /// Defines the contractaccount.
        /// </summary>
        [JsonProperty("contractaccount")]
        public string contractaccount;

        /// <summary>
        /// Defines the sessionid.
        /// </summary>
        [JsonProperty("sessionid")]
        public string sessionid;

        /// <summary>
        /// Defines the vendorid.
        /// </summary>
        [JsonProperty("vendorid")]
        public string vendorid;

        /// <summary>
        /// Defines the mobileosversion.
        /// </summary>
        [JsonProperty("mobileosversion")]
        public string mobileosversion;

        /// <summary>
        /// Defines the appidentifier.
        /// </summary>
        [JsonProperty("appidentifier")]
        public string appidentifier;

        /// <summary>
        /// Defines the appversion.
        /// </summary>
        [JsonProperty("appversion")]
        public string appversion;

        /// <summary>
        /// Defines the lang.
        /// </summary>
        [JsonProperty("lang")]
        public string lang;

        /// <summary>
        /// Defines the activeflag.
        /// </summary>
        [JsonProperty("activeflag")]
        public string activeflag;
    }
}
