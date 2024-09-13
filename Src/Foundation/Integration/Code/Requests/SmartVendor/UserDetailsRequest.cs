// <copyright file="UserDetailsRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartVendor
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Icainquirydetails" />.
    /// </summary>
    public class Icainquirydetails
    {
        /// <summary>
        /// Gets or sets the dateofbirth.
        /// </summary>
        [JsonProperty("dateofbirth")]
        public string dateofbirth { get; set; }

        /// <summary>
        /// Gets or sets the emiratesid.
        /// </summary>
        [JsonProperty("emiratesid")]
        public string emiratesid { get; set; }

        /// <summary>
        /// Gets or sets the emiratesidexpirydate.
        /// </summary>
        [JsonProperty("emiratesidexpirydate")]
        public string emiratesidexpirydate { get; set; }

        /// <summary>
        /// Gets or sets the unifiednumber.
        /// </summary>
        [JsonProperty("unifiednumber")]
        public string unifiednumber { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Rtainquirydetails" />.
    /// </summary>
    public class Rtainquirydetails
    {
        /// <summary>
        /// Gets or sets the platecategory.
        /// </summary>
        [JsonProperty("platecategory")]
        public string platecategory { get; set; }

        /// <summary>
        /// Gets or sets the platecode.
        /// </summary>
        [JsonProperty("platecode")]
        public string platecode { get; set; }

        /// <summary>
        /// Gets or sets the platenumber.
        /// </summary>
        [JsonProperty("platenumber")]
        public string platenumber { get; set; }

        /// <summary>
        /// Gets or sets the platesource.
        /// </summary>
        [JsonProperty("platesource")]
        public string platesource { get; set; }

        /// <summary>
        /// Gets or sets the registrationstatus.
        /// </summary>
        [JsonProperty("registrationstatus")]
        public string registrationstatus { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Inquiryinput" />.
    /// </summary>
    public class Inquiryinput
    {
        /// <summary>
        /// Gets or sets the appidentifier.
        /// </summary>
        [JsonProperty("appidentifier")]
        public string appidentifier { get; set; }

        /// <summary>
        /// Gets or sets the appversion.
        /// </summary>
        [JsonProperty("appversion")]
        public string appversion { get; set; }

        /// <summary>
        /// Gets or sets the icainquirydetails.
        /// </summary>
        [JsonProperty("icainquirydetails")]
        public Icainquirydetails icainquirydetails { get; set; }

        /// <summary>
        /// Gets or sets the icartaflag.
        /// </summary>
        [JsonProperty("icartaflag")]
        public string icartaflag { get; set; }

        /// <summary>
        /// Gets or sets the lang.
        /// </summary>
        [JsonProperty("lang")]
        public string lang { get; set; }

        /// <summary>
        /// Gets or sets the mobileosversion.
        /// </summary>
        [JsonProperty("mobileosversion")]
        public string mobileosversion { get; set; }

        /// <summary>
        /// Gets or sets the rtainquirydetails.
        /// </summary>
        [JsonProperty("rtainquirydetails")]
        public Rtainquirydetails rtainquirydetails { get; set; }

        /// <summary>
        /// Gets or sets the sessionid.
        /// </summary>
        [JsonProperty("sessionid")]
        public string sessionid { get; set; }

        /// <summary>
        /// Gets or sets the userid.
        /// </summary>
        [JsonProperty("userid")]
        public string userid { get; set; }

        /// <summary>
        /// Gets or sets the vendorid.
        /// </summary>
        [JsonProperty("vendorid")]
        public string vendorid { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="UserDetailsRequest" />.
    /// </summary>
    public class UserDetailsRequest
    {
        /// <summary>
        /// Gets or sets the inquiryinput.
        /// </summary>
        [JsonProperty("inquiryinput")]
        public Inquiryinput inquiryinput { get; set; }
    }
}
