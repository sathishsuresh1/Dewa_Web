// <copyright file="SendOTPResponse.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVSmartCharger
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="OneTimeChargeResponse" />.
    /// </summary>
    public class OneTimeChargeResponse
    {
        /// <summary>
        /// Gets or sets the Connectorid.
        /// </summary>
        [JsonProperty("connectorid")]
        public string Connectorid { get; set; }

        /// <summary>
        /// Gets or sets the Dateandtime.
        /// </summary>
        [JsonProperty("dateandtime")]
        public string Dateandtime { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Devicedbid.
        /// </summary>
        [JsonProperty("devicedbid")]
        public string Devicedbid { get; set; }

        /// <summary>
        /// Gets or sets the Devicetype.
        /// </summary>
        [JsonProperty("devicetype")]
        public string Devicetype { get; set; }

        /// <summary>
        /// Gets or sets the Evamount.
        /// </summary>
        [JsonProperty("evamount")]
        public string Evamount { get; set; }

        /// <summary>
        /// Gets or sets the Evchargeend.
        /// </summary>
        [JsonProperty("evchargeend")]
        public string Evchargeend { get; set; }

        /// <summary>
        /// Gets or sets the Evchargestart.
        /// </summary>
        [JsonProperty("evchargestart")]
        public string Evchargestart { get; set; }

        /// <summary>
        /// Gets or sets the Evrfid.
        /// </summary>
        [JsonProperty("evrfid")]
        public string Evrfid { get; set; }

        /// <summary>
        /// Gets or sets the Evstatus.
        /// </summary>
        [JsonProperty("evstatus")]
        public string Evstatus { get; set; }

        /// <summary>
        /// Gets or sets the Locationaddress.
        /// </summary>
        [JsonProperty("locationaddress")]
        public string Locationaddress { get; set; }

        /// <summary>
        /// Gets or sets the EVduraion.
        /// </summary>
        [JsonProperty("evduration")]
        public string EVduraion { get; set; }

        /// <summary>
        /// Gets or sets the Otcpacks.
        /// </summary>
        [JsonProperty("otcpacks")]
        public List<Otcpack> Otcpacks { get; set; }

        /// <summary>
        /// Gets or sets the Otcpayment.
        /// </summary>
        [JsonProperty("otcpayment")]
        public List<Otcpayment> Otcpayment { get; set; }

        /// <summary>
        /// Gets or sets the Packagename.
        /// </summary>
        [JsonProperty("packagename")]
        public string Packagename { get; set; }

        /// <summary>
        /// Gets or sets the Requestid.
        /// </summary>
        [JsonProperty("requestid")]
        public string Requestid { get; set; }

        /// <summary>
        /// Gets or sets the Responsecode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }

        /// <summary>
        /// Gets or sets the Maxotp.
        /// </summary>
        [JsonProperty("maxotp")]
        public bool Maxotp { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Otcpack" />.
    /// </summary>
    public class Otcpack
    {
        /// <summary>
        /// Gets or sets the Currency.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the Packageamount.
        /// </summary>
        [JsonProperty("packageamount")]
        public string Packageamount { get; set; }

        /// <summary>
        /// Gets or sets the Packname.
        /// </summary>
        [JsonProperty("packname")]
        public string Packname { get; set; }

        /// <summary>
        /// Gets or sets the Packtype.
        /// </summary>
        [JsonProperty("packtype")]
        public string Packtype { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Otcpayment" />.
    /// </summary>
    public class Otcpayment
    {
        /// <summary>
        /// Gets or sets the Amount.
        /// </summary>
        [JsonProperty("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Gets or sets the Contractaccount.
        /// </summary>
        [JsonProperty("contractaccount")]
        public string Contractaccount { get; set; }

        /// <summary>
        /// Gets or sets the Legacyaccount.
        /// </summary>
        [JsonProperty("legacyaccount")]
        public string Legacyaccount { get; set; }

        /// <summary>
        /// Gets or sets the Service.
        /// </summary>
        [JsonProperty("service")]
        public object Service { get; set; }
    }
}
