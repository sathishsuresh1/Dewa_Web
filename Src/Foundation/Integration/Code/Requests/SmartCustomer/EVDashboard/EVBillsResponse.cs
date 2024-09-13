// <copyright file="EVBillsResponse.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Outstandingbreakdown" />.
    /// </summary>
    public class Outstandingbreakdown
    {
        /// <summary>
        /// Gets or sets the businesspartner.
        /// </summary>
        [JsonProperty("businesspartner")]
        public string businesspartner { get; set; }

        /// <summary>
        /// Gets or sets the connectionstatus.
        /// </summary>
        [JsonProperty("connectionstatus")]
        public string connectionstatus { get; set; }

        /// <summary>
        /// Gets or sets the contractaccount.
        /// </summary>
        [JsonProperty("contractaccount")]
        public string contractaccount { get; set; }

        /// <summary>
        /// Gets or sets the cooling.
        /// </summary>
        [JsonProperty("cooling")]
        public decimal cooling { get; set; }

        /// <summary>
        /// Gets or sets the discount.
        /// </summary>
        [JsonProperty("discount")]
        public string discount { get; set; }

        /// <summary>
        /// Gets or sets the dm.
        /// </summary>
        [JsonProperty("dm")]
        public decimal dm { get; set; }

        /// <summary>
        /// Gets or sets the electricity.
        /// </summary>
        [JsonProperty("electricity")]
        public decimal electricity { get; set; }

        /// <summary>
        /// Gets or sets the finalbill.
        /// </summary>
        [JsonProperty("finalbill")]
        public string finalbill { get; set; }

        /// <summary>
        /// Gets or sets the housing.
        /// </summary>
        [JsonProperty("housing")]
        public decimal housing { get; set; }

        /// <summary>
        /// Gets or sets the indicator.
        /// </summary>
        [JsonProperty("indicator")]
        public string indicator { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty("name")]
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the nickname.
        /// </summary>
        [JsonProperty("nickname")]
        public string nickname { get; set; }

        /// <summary>
        /// Gets or sets the others.
        /// </summary>
        [JsonProperty("others")]
        public decimal others { get; set; }

        /// <summary>
        /// Gets or sets the payprocess.
        /// </summary>
        [JsonProperty("payprocess")]
        public string payprocess { get; set; }

        /// <summary>
        /// Gets or sets the sewerage.
        /// </summary>
        [JsonProperty("sewerage")]
        public decimal sewerage { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        [JsonProperty("total")]
        public decimal total { get; set; }

        /// <summary>
        /// Gets or sets the water.
        /// </summary>
        [JsonProperty("water")]
        public decimal water { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="EVBillsResponse" />.
    /// </summary>
    public class EVBillsResponse
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Outstandingbreakdown.
        /// </summary>
        [JsonProperty("outstandingbreakdown")]
        public List<Outstandingbreakdown> Outstandingbreakdown { get; set; }

        /// <summary>
        /// Gets or sets the Responsecode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }
    }
}
