using System;
// <copyright file="DewaRestClient.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartCustomer
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    // <summary>
    /// Defines the <see cref="slabTarrifResponse" />.
    /// </summary>
    public class slabTarrifResponse
    {
        /// <summary>
        /// Define response code
        /// </summary>
        [JsonProperty("responseCode")]
        public string responseCode;
        /// <summary>
        /// Define response message
        /// </summary>
        [JsonProperty("responseMessage")]
        public string responseMessage;
        /// <summary>
        /// Define slab caps
        /// </summary>
        [JsonProperty("slabCaps")]
        public slabCap[] slabCaps;
    }
    // <summary>
    /// Defines the <see cref="slabCap" />.
    /// </summary>
    public class slabCap
    {
        /// <summary>
        /// Define contract account
        /// </summary>
        [JsonProperty("contractAccount")]
        public string contractAccount { get; set; }
        /// <summary>
        /// Define division
        /// </summary>
        [JsonProperty("division")]
        public string division { get; set; }
        /// <summary>
        /// Define slab1
        /// </summary>
        [JsonProperty("slab1")]
        /// <summary>
        /// Define slab2
        /// </summary>
        public string slab1 { get; set; }
        [JsonProperty("slab2")]
        public string slab2 { get; set; }
        /// <summary>
        /// Define slab3
        /// </summary>
        [JsonProperty("slab3")]
        public string slab3 { get; set; }
        /// <summary>
        /// Define slab4
        /// </summary>
        [JsonProperty("slab4")]
        public string slab4 { get; set; }
        /// <summary>
        /// Define slab5
        /// </summary>
        [JsonProperty("slab5")]
        public string slab5 { get; set; }
    }
}
