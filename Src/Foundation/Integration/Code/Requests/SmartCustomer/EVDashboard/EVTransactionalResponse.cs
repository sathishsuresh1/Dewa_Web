// <copyright file="EVTransactionalResponse.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Transactiondetail" />.
    /// </summary>
    public class Transactiondetail
    {
        /// <summary>
        /// Gets or sets the Amount.
        /// </summary>
        [JsonProperty("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Gets or sets the Cardnumber.
        /// </summary>
        [JsonProperty("cardnumber")]
        public string Cardnumber { get; set; }

        /// <summary>
        /// Gets or sets the Consumption.
        /// </summary>
        [JsonProperty("consumption")]
        public string Consumption { get; set; }

        /// <summary>
        /// Gets or sets the Descriptionforcharges.
        /// </summary>
        [JsonProperty("descriptionforcharges")]
        public string Descriptionforcharges { get; set; }

        /// <summary>
        /// Gets or sets the Discount.
        /// </summary>
        [JsonProperty("discount")]
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets the Duration.
        /// </summary>
        [JsonProperty("duration")]
        public string Duration { get; set; }

        /// <summary>
        /// Gets or sets the Durationinseconds.
        /// </summary>
        [JsonProperty("durationinseconds")]
        public int Durationinseconds { get; set; }

        /// <summary>
        /// Gets or sets the Locationname.
        /// </summary>
        [JsonProperty("locationname")]
        public string Locationname { get; set; }

        /// <summary>
        /// Gets or sets the Platenumber.
        /// </summary>
        [JsonProperty("platenumber")]
        public string Platenumber { get; set; }

        /// <summary>
        /// Gets or sets the Salesunit.
        /// </summary>
        [JsonProperty("salesunit")]
        public string Salesunit { get; set; }

        /// <summary>
        /// Gets or sets the Sddocumentcurrency.
        /// </summary>
        [JsonProperty("sddocumentcurrency")]
        public string Sddocumentcurrency { get; set; }

        /// <summary>
        /// Gets or sets the Transactiondate.
        /// </summary>
        [JsonProperty("transactiondate")]
        public string Transactiondate { get; set; }

        /// <summary>
        /// Gets or sets the Transactionid.
        /// </summary>
        [JsonProperty("transactionid")]
        public string Transactionid { get; set; }

        /// <summary>
        /// Gets or sets the Transactiontime.
        /// </summary>
        [JsonProperty("transactiontime")]
        public string Transactiontime { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="EVTransactionalResponse" />.
    /// </summary>
    public class EVTransactionalResponse
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Discountamount.
        /// </summary>
        [JsonProperty("discountamount")]
        public string Discountamount { get; set; }

        /// <summary>
        /// Gets or sets the Responsecode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }

        /// <summary>
        /// Gets or sets the Salesunit.
        /// </summary>
        [JsonProperty("salesunit")]
        public string Salesunit { get; set; }

        /// <summary>
        /// Gets or sets the Sddcoumentcurrency.
        /// </summary>
        [JsonProperty("sddcoumentcurrency")]
        public string Sddcoumentcurrency { get; set; }

        /// <summary>
        /// Gets or sets the Totalconsumption.
        /// </summary>
        [JsonProperty("totalconsumption")]
        public string Totalconsumption { get; set; }

        /// <summary>
        /// Gets or sets the Totalcost.
        /// </summary>
        [JsonProperty("totalcost")]
        public string Totalcost { get; set; }

        /// <summary>
        /// Gets or sets the Transactiondetails.
        /// </summary>
        [JsonProperty("transactiondetails")]
        public List<Transactiondetail> Transactiondetails { get; set; }
    }
}
