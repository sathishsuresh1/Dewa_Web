// <copyright file="EVConsumptionResponse.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Defines the <see cref="Dailyconsumption" />.
    /// </summary>
    public class Dailyconsumption
    {
        /// <summary>
        /// Gets or sets the Billingkeydate.
        /// </summary>
        [JsonProperty("billingkeydate")]
        public string Billingkeydate { get; set; }

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
        /// Gets or sets the Contractaccount.
        /// </summary>
        [JsonProperty("contractaccount")]
        public string Contractaccount { get; set; }

        /// <summary>
        /// Gets or sets the Month.
        /// </summary>
        [JsonProperty("month")]
        public string Month { get; set; }

        /// <summary>
        /// Gets or sets the Salesunit.
        /// </summary>
        [JsonProperty("salesunit")]
        public string Salesunit { get; set; }

        /// <summary>
        /// Gets or sets the Transactiondate.
        /// </summary>
        [JsonProperty("transactiondate")]
        public string Transactiondate { get; set; }

        /// <summary>
        /// Gets or sets the Year.
        /// </summary>
        [JsonProperty("year")]
        public string Year { get; set; }

        /// <summary>
        /// Gets the Period.
        /// </summary>
        public DateTime Period => DateTime.ParseExact(Billingkeydate, "yyyy/MM", CultureInfo.InvariantCulture);
        public DateTime TransactiondatePeriod => DateTime.ParseExact(Transactiondate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        
    }


    public class Consumptions
    {
        [JsonProperty("billingkeydate")]
        public string Billingkeydate { get; set; }

        [JsonProperty("cardnumber")]
        public string Cardnumber { get; set; }

        [JsonProperty("consumption")]
        public string Consumption { get; set; }

        [JsonProperty("contractaccount")]
        public string Contractaccount { get; set; }

        [JsonProperty("month")]
        public string Month { get; set; }

        [JsonProperty("salesunit")]
        public string Salesunit { get; set; }

        [JsonProperty("transactiondate")]
        public string Transactiondate { get; set; }
        public DateTime Period => DateTime.ParseExact(Billingkeydate, "yyyy/MM", CultureInfo.InvariantCulture);
        public DateTime TransactiondatePeriod => DateTime.ParseExact(Transactiondate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    public class Lstconsumption
    {
        [JsonProperty("consumptions")]
        public List<Consumptions> Consumption { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }
    }

    public class EVConsumptionResponse
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("electricityconsumption")]
        public List<Lstconsumption> Electricityconsumption { get; set; }
        [JsonProperty("dailyconsumption")]
        public List<Lstconsumption> Dailyconsumption { get; set; }

        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }
    }



}
