// <copyright file="TradeLicenseResponse.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartVendor
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using SitecoreX = Sitecore.Context;

    /// <summary>
    /// Defines the <see cref="TradeLicenseResponse" />.
    /// </summary>
    public class TradeLicenseResponse
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Responsecode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }

        /// <summary>
        /// Gets or sets the Tradeexistflag.
        /// </summary>
        [JsonProperty("tradeexistflag")]
        public string Tradeexistflag { get; set; }

        /// <summary>
        /// Gets or sets the Tradelicenseenddate.
        /// </summary>
        [JsonProperty("tradelicenseenddate")]
        public string Tradelicenseenddate { get; set; }

        /// <summary>
        /// Gets or sets the Tradelicensestartdate.
        /// </summary>
        [JsonProperty("tradelicensestartdate")]
        public string Tradelicensestartdate { get; set; }
        public string formattradelicenseenddate
        {
            get
            {
                if (!string.IsNullOrEmpty(Tradelicenseenddate) && !Tradelicenseenddate.Equals("0000-00-00"))
                {
                    return DateTime.ParseExact(Tradelicenseenddate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", SitecoreX.Culture);
                }
                return string.Empty;
            }
        }
        public string formattradelicensestartdate
        {
            get
            {
                if (!string.IsNullOrEmpty(Tradelicensestartdate) && !Tradelicensestartdate.Equals("0000-00-00"))
                {
                    return DateTime.ParseExact(Tradelicensestartdate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", SitecoreX.Culture);
                }
                return string.Empty;
            }
        }
    }
}
