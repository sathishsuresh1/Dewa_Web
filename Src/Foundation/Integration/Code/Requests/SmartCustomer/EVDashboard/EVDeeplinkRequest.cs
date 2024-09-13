// <copyright file="EVDeeplinkRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="EVDeeplinkRequest" />.
    /// </summary>
    public class EVDeeplinkRequest
    {
        /// <summary>
        /// Gets or sets the vendorid.
        /// </summary>
        [JsonProperty("vendorid")]
        public string vendorid { get; set; }

        /// <summary>
        /// Gets or sets the param.
        /// </summary>
        [JsonProperty("param")]
        public string param { get; set; }

        /// <summary>
        /// Gets or sets the userid.
        /// </summary>
        [JsonProperty("userid")]
        public string userid { get; set; }

        /// <summary>
        /// Gets or sets the mobileosversion.
        /// </summary>
        [JsonProperty("mobileosversion")]
        public string mobileosversion { get; set; }

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
    }
}
