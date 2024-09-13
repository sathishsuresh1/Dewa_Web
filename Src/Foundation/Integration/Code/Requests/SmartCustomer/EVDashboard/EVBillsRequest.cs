// <copyright file="EVBillsRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="OutstandingbreakdownIN" />.
    /// </summary>
    public class OutstandingbreakdownIN
    {
        /// <summary>
        /// Gets or sets the contractaccount.
        /// </summary>
        [JsonProperty("contractaccount")]
        public string contractaccount { get; set; }

        /// <summary>
        /// Gets or sets the lang.
        /// </summary>
        [JsonProperty("lang")]
        public string lang { get; set; }

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        [JsonProperty("center")]
        public string center { get; set; }

        /// <summary>
        /// Gets or sets the vendorid.
        /// </summary>
        [JsonProperty("vendorid")]
        public string vendorid { get; set; }

        /// <summary>
        /// Gets or sets the sessionid.
        /// </summary>
        [JsonProperty("sessionid")]
        public string sessionid { get; set; }

        /// <summary>
        /// Gets or sets the xcoordinate.
        /// </summary>
        [JsonProperty("xcoordinate")]
        public object xcoordinate { get; set; }

        /// <summary>
        /// Gets or sets the ycoordinate.
        /// </summary>
        [JsonProperty("ycoordinate")]
        public object ycoordinate { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="EVBillsRequest" />.
    /// </summary>
    public class EVBillsRequest
    {
        /// <summary>
        /// Gets or sets the outstandingbreakdownIN.
        /// </summary>
        [JsonProperty("outstandingbreakdownIN")]
        public OutstandingbreakdownIN outstandingbreakdownIN { get; set; }
    }
}
