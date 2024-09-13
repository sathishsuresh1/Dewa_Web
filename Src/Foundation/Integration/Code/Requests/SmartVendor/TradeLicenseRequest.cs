// <copyright file="TradeLicenseInput.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartVendor
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Tradedetailsinput" />.
    /// </summary>
    public class Tradedetailsinput
    {
        /// <summary>
        /// Gets or sets the appidentifier.
        /// </summary>
        [JsonProperty("appidentifier")]
        public string appidentifier { get; set; }

        /// <summary>
        /// Gets or sets the appversion.
        /// </summary>
        [JsonProperty("appversion")]
        public string appversion { get; set; }

        /// <summary>
        /// Gets or sets the lang.
        /// </summary>
        [JsonProperty("lang")]
        public string lang { get; set; }

        /// <summary>
        /// Gets or sets the mobileosversion.
        /// </summary>
        [JsonProperty("mobileosversion")]
        public string mobileosversion { get; set; }

        /// <summary>
        /// Gets or sets the sessionid.
        /// </summary>
        [JsonProperty("sessionid")]
        public string sessionid { get; set; }

        /// <summary>
        /// Gets or sets the tradelicensenumber.
        /// </summary>
        [JsonProperty("tradelicensenumber")]
        public string tradelicensenumber { get; set; }

        /// <summary>
        /// Gets or sets the userid.
        /// </summary>
        [JsonProperty("userid")]
        public string userid { get; set; }

        /// <summary>
        /// Gets or sets the vendorid.
        /// </summary>
        [JsonProperty("vendorid")]
        public string vendorid { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="TradeLicenseInput" />.
    /// </summary>
    public class TradeLicenseRequest
    {
        /// <summary>
        /// Gets or sets the tradedetailsinput.
        /// </summary>
        [JsonProperty("tradedetailsinput")]
        public Tradedetailsinput tradedetailsinput { get; set; }
    }
}
