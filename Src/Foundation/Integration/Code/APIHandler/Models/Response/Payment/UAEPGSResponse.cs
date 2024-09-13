// <copyright file="UAEPGSResponse.cs">
// Copyright (c) 2023
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Payment
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="UAEPGSResponse" />.
    /// </summary>
    public class UAEPGSResponse
    {
        /// <summary>
        /// Defines the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description;

        /// <summary>
        /// Defines the Responsecode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string Responsecode;

        /// <summary>
        /// Defines the Uaepgsdetails.
        /// </summary>
        [JsonProperty("uaepgsdetails")]
        public List<Uaepgsdetail> Uaepgsdetails;
    }

    /// <summary>
    /// Defines the <see cref="Uaepgsdetail" />.
    /// </summary>
    public class Uaepgsdetail
    {
        /// <summary>
        /// Defines the Bankkey.
        /// </summary>
        [JsonProperty("bankkey")]
        public string Bankkey;

        /// <summary>
        /// Defines the Bankname.
        /// </summary>
        [JsonProperty("bankname")]
        public string Bankname;

        /// <summary>
        /// Defines the Imagelink.
        /// </summary>
        [JsonProperty("imagelink")]
        public string Imagelink;

        /// <summary>
        /// Defines the Url.
        /// </summary>
        [JsonProperty("url")]
        public string Url;
    }
}
