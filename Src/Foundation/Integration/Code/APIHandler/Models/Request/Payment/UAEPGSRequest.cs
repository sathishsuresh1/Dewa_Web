// <copyright file="UAEPGSRequest.cs">
// Copyright (c) 2023
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Payment
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="UAEPGSRequest" />.
    /// </summary>
    public class UAEPGSRequest
    {
        /// <summary>
        /// Defines the vendorid.
        /// </summary>
        [JsonProperty("vendorid")]
        public string vendorid;

        /// <summary>
        /// Defines the lang.
        /// </summary>
        [JsonProperty("lang")]
        public string lang;
    }
}
