// <copyright file="UserDetailsResponse.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartVendor
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using SitecoreX = Sitecore.Context;

    /// <summary>
    /// Defines the <see cref="Icadetails" />.
    /// </summary>
    public class Icadetails
    {
        [JsonProperty("companyname")]
        public string CompanyName { get; set; }
        /// <summary>
        /// Gets or sets the Fullname.
        /// </summary>
        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        /// <summary>
        /// Gets or sets the Nationality.
        /// </summary>
        [JsonProperty("nationality")]
        public string Nationality { get; set; }
        [JsonProperty("profession")]
        public string Profession { get; set; }

        /// <summary>
        /// Gets or sets the Passportexpirydate.
        /// </summary>
        [JsonProperty("passportexpirydate")]
        public string Passportexpirydate { get; set; }

        /// <summary>
        /// Gets or sets the Passportnumber.
        /// </summary>
        [JsonProperty("passportnumber")]
        public string Passportnumber { get; set; }

        /// <summary>
        /// Gets or sets the Visaexpirydate.
        /// </summary>
        [JsonProperty("visaexpirydate")]
        public string Visaexpirydate { get; set; }

        /// <summary>
        /// Gets or sets the Visanumber.
        /// </summary>
        [JsonProperty("visanumber")]
        public string Visanumber { get; set; }
        public string formatvisanumber
        {
            get
            {
                if (!string.IsNullOrEmpty(Visanumber))
                {
                    return Visanumber.Replace("/", string.Empty);
                }
                return Visanumber;
            }
        }

        public string formatvisaexpirydate
        {
            get
            {
                if (!string.IsNullOrEmpty(Visaexpirydate) && !Visaexpirydate.Equals("0000-00-00"))
                {
                    return DateTime.ParseExact(Visaexpirydate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", SitecoreX.Culture);
                }
                return string.Empty;
            }
        }

        public string formatpassportexpirydate
        {
            get
            {
                if (!string.IsNullOrEmpty(Passportexpirydate) && !Passportexpirydate.Equals("0000-00-00"))
                {
                    return DateTime.ParseExact(Passportexpirydate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", SitecoreX.Culture);
                }
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Defines the <see cref="Rtadetails" />.
    /// </summary>
    public class Rtadetails
    {
        /// <summary>
        /// Gets or sets the Emiratesid.
        /// </summary>
        [JsonProperty("emiratesid")]
        public string Emiratesid { get; set; }

        /// <summary>
        /// Gets or sets the Passportnumber.
        /// </summary>
        [JsonProperty("passportnumber")]
        public string Passportnumber { get; set; }
        [JsonProperty("insuranceexpirydate")]
        public string Insuranceexpirydate { get; set; }
        public string formatinsuranceexpirydate
        {
            get
            {
                if (!string.IsNullOrEmpty(Insuranceexpirydate) && !Insuranceexpirydate.Equals("0000-00-00"))
                {
                    return DateTime.ParseExact(Insuranceexpirydate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", SitecoreX.Culture);
                }
                return string.Empty;
            }
        }
        [JsonProperty("registrationdate")]
        public string Registrationdate { get; set; }
        public string formatregistrationdate
        {
            get
            {
                if (!string.IsNullOrEmpty(Registrationdate) && !Registrationdate.Equals("0000-00-00"))
                {
                    return DateTime.ParseExact(Registrationdate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", SitecoreX.Culture);
                }
                return string.Empty;
            }
        }
        [JsonProperty("expirydate")]
        public string Expirydate { get; set; }
        public string formatexpirydate
        {
            get
            {
                if (!string.IsNullOrEmpty(Expirydate) && !Expirydate.Equals("0000-00-00"))
                {
                    return DateTime.ParseExact(Expirydate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", SitecoreX.Culture);
                }
                return string.Empty;
            }
        }

    }

    /// <summary>
    /// Defines the <see cref="UserDetailsResponse" />.
    /// </summary>
    public class UserDetailsResponse
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Icadetails.
        /// </summary>
        [JsonProperty("icadetails")]
        public Icadetails Icadetails { get; set; }

        /// <summary>
        /// Gets or sets the Responsecode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }

        /// <summary>
        /// Gets or sets the Rtadetails.
        /// </summary>
        [JsonProperty("rtadetails")]
        public Rtadetails Rtadetails { get; set; }
    }
}
