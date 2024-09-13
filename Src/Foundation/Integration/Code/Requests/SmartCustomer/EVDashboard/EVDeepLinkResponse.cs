// <copyright file="EVDeepLinkResponse.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Vehiclelist" />.
    /// </summary>
    public class Vehiclelist
    {
        /// <summary>
        /// Gets or sets the Courierfee.
        /// </summary>
        [JsonProperty("courierfee")]
        public string Courierfee { get; set; }

        /// <summary>
        /// Gets or sets the Courierfeevatamount.
        /// </summary>
        [JsonProperty("courierfeevatamount")]
        public string Courierfeevatamount { get; set; }

        /// <summary>
        /// Gets or sets the Currencykey.
        /// </summary>
        [JsonProperty("currencykey")]
        public string Currencykey { get; set; }

        /// <summary>
        /// Gets or sets the Platecategorycode.
        /// </summary>
        [JsonProperty("platecategorycode")]
        public string Platecategorycode { get; set; }

        /// <summary>
        /// Gets or sets the Platecategorydescription.
        /// </summary>
        [JsonProperty("platecategorydescription")]
        public string Platecategorydescription { get; set; }

        /// <summary>
        /// Gets or sets the Platecode.
        /// </summary>
        [JsonProperty("platecode")]
        public string Platecode { get; set; }

        /// <summary>
        /// Gets or sets the Platecodedescription.
        /// </summary>
        [JsonProperty("platecodedescription")]
        public string Platecodedescription { get; set; }

        /// <summary>
        /// Gets or sets the Platenumber.
        /// </summary>
        [JsonProperty("platenumber")]
        public string Platenumber { get; set; }

        /// <summary>
        /// Gets or sets the Sdamount.
        /// </summary>
        [JsonProperty("sdamount")]
        public string Sdamount { get; set; }

        /// <summary>
        /// Gets or sets the Totalamount.
        /// </summary>
        [JsonProperty("totalamount")]
        public string Totalamount { get; set; }

        /// <summary>
        /// Gets or sets the Vatrate.
        /// </summary>
        [JsonProperty("vatrate")]
        public string Vatrate { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="EVDeepLinkResponse" />.
    /// </summary>
    public class EVDeepLinkResponse
    {
        /// <summary>
        /// Gets or sets the Businesspartnernumber.
        /// </summary>
        [JsonProperty("businesspartnernumber")]
        public string Businesspartnernumber { get; set; }

        /// <summary>
        /// Gets or sets the Carregistrationcountry.
        /// </summary>
        [JsonProperty("carregistrationcountry")]
        public string Carregistrationcountry { get; set; }

        /// <summary>
        /// Gets or sets the Carregistrationregion.
        /// </summary>
        [JsonProperty("carregistrationregion")]
        public string Carregistrationregion { get; set; }

        /// <summary>
        /// Gets or sets the Customercategory.
        /// </summary>
        [JsonProperty("customercategory")]
        public string Customercategory { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Fullname.
        /// </summary>
        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        /// <summary>
        /// Gets or sets the Responsecode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }

        /// <summary>
        /// Gets or sets the Trafficfileno.
        /// </summary>
        [JsonProperty("trafficfileno")]
        public string Trafficfileno { get; set; }

        /// <summary>
        /// Gets or sets the Userid.
        /// </summary>
        [JsonProperty("userid")]
        public string Userid { get; set; }

        /// <summary>
        /// Gets or sets the Vehiclelist.
        /// </summary>
        [JsonProperty("vehiclelist")]
        public List<Vehiclelist> Vehiclelist { get; set; }
    }
}
