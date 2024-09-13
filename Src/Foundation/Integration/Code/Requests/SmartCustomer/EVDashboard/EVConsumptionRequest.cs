// <copyright file="EVConsumptionRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="EVConsumptionRequest" />.
    /// </summary>
    public class EVConsumptionRequest
    {
        /// <summary>
        /// Gets or sets the contractaccount.
        /// </summary>
        [JsonProperty("contractaccount")]
        public string contractaccount { get; set; }

        /// <summary>
        /// Gets or sets the sessionid.
        /// </summary>
        [JsonProperty("sessionid")]
        public string sessionid { get; set; }

        /// <summary>
        /// Gets or sets the cardnumber.
        /// </summary>
        [JsonProperty("cardnumber")]
        public string cardnumber { get; set; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        [JsonProperty("year")]
        public string year { get; set; }

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        [JsonProperty("month")]
        public string month { get; set; }

        /// <summary>
        /// Gets or sets the vendorid.
        /// </summary>
        [JsonProperty("vendorid")]
        public string vendorid { get; set; }

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
