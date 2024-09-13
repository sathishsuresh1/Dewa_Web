// <copyright file="SendOTPRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVSmartCharger
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Chargein" />.
    /// </summary>
    public class Chargein
    {
        /// <summary>
        /// Gets or sets the lang.
        /// </summary>
        [JsonProperty("lang")]
        public string lang { get; set; }

        /// <summary>
        /// Gets or sets the vendorid.
        /// </summary>
        [JsonProperty("vendorid")]
        public string vendorid { get; set; }

        /// <summary>
        /// Gets or sets the evmode.
        /// </summary>
        [JsonProperty("evmode")]
        public string evmode { get; set; }

        /// <summary>
        /// Gets or sets the devicedbid.
        /// </summary>
        [JsonProperty("devicedbid")]
        public string devicedbid { get; set; }

        /// <summary>
        /// Gets or sets the devicetype.
        /// </summary>
        [JsonProperty("devicetype")]
        public string devicetype { get; set; }

        /// <summary>
        /// Gets or sets the connectorid.
        /// </summary>
        [JsonProperty("connectorid")]
        public string connectorid { get; set; }

        /// <summary>
        /// Gets or sets the locationaddress.
        /// </summary>
        [JsonProperty("locationaddress")]
        public string locationaddress { get; set; }

        /// <summary>
        /// Gets or sets the fullname.
        /// </summary>
        [JsonProperty("fullname")]
        public string fullname { get; set; }

        /// <summary>
        /// Gets or sets the contactemail.
        /// </summary>
        [JsonProperty("contactemail")]
        public string contactemail { get; set; }

        /// <summary>
        /// Gets or sets the contactmobile.
        /// </summary>
        [JsonProperty("contactmobile")]
        public string contactmobile { get; set; }

        /// <summary>
        /// Gets or sets the urlparameter.
        /// </summary>
        [JsonProperty("urlparameter")]
        public string urlparameter { get; set; }

        /// <summary>
        /// Gets or sets the vehicleidentification.
        /// </summary>
        [JsonProperty("vehicleidentification")]
        public string vehicleidentification { get; set; }

        /// <summary>
        /// Gets or sets the requestid.
        /// </summary>
        [JsonProperty("requestid")]
        public string requestid { get; set; }

        /// <summary>
        /// Gets or sets the otp.
        /// </summary>
        [JsonProperty("otp")]
        public string otp { get; set; }

        /// <summary>
        /// Gets or sets the packagetype.
        /// </summary>
        [JsonProperty("packagetype")]
        public string packagetype { get; set; }

        /// <summary>
        /// Gets or sets the vatnumber.
        /// </summary>
        [JsonProperty("vatnumber")]
        public string vatnumber { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="OneTimeChargeRequest" />.
    /// </summary>
    public class OneTimeChargeRequest
    {
        /// <summary>
        /// Gets or sets the chargein.
        /// </summary>
        [JsonProperty("chargein")]
        public Chargein chargein { get; set; }
    }
}
