// <copyright file="BillHistoryRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="BillHistoryRequest" />.
    /// </summary>
    public class BillHistoryRequest
    {
        /// <summary>
        /// Gets or sets the userid.
        /// </summary>
        [JsonProperty("userid")]
        public string userid { get; set; }

        /// <summary>
        /// Gets or sets the sessionid.
        /// </summary>
        [JsonProperty("sessionid")]
        public string sessionid { get; set; }

        /// <summary>
        /// Gets or sets the contractaccountnumber.
        /// </summary>
        [JsonProperty("contractaccountnumber")]
        public string contractaccountnumber { get; set; }

        /// <summary>
        /// Gets or sets the servicetype.
        /// </summary>
        [JsonProperty("servicetype")]
        public string servicetype { get; set; }

        /// <summary>
        /// Gets or sets the lang.
        /// </summary>
        [JsonProperty("lang")]
        public string lang { get; set; }

        /// <summary>
        /// Gets or sets the estimatenumber.
        /// </summary>
        [JsonProperty("estimatenumber")]
        public string estimatenumber { get; set; }
    }
}
