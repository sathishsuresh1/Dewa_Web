// <copyright file="LoginRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Getloginsessionrequest" />.
    /// </summary>
    public class Getloginsessionrequest
    {
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [JsonProperty("password")]
        public string password { get; set; }

        /// <summary>
        /// Gets or sets the merchantid.
        /// </summary>
        [JsonProperty("merchantid")]
        public string merchantid { get; set; }

        /// <summary>
        /// Gets or sets the merchantpassword.
        /// </summary>
        [JsonProperty("merchantpassword")]
        public string merchantpassword { get; set; }

        /// <summary>
        /// Gets or sets the userid.
        /// </summary>
        [JsonProperty("userid")]
        public string userid { get; set; }

        /// <summary>
        /// Gets or sets the appversion.
        /// </summary>
        [JsonProperty("appversion")]
        public string appversion { get; set; }

        /// <summary>
        /// Gets or sets the appidentifier.
        /// </summary>
        [JsonProperty("appidentifier")]
        public string appidentifier { get; set; }

        /// <summary>
        /// Gets or sets the lang.
        /// </summary>
        [JsonProperty("lang")]
        public string lang { get; set; }

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        [JsonProperty("center")]
        public string center { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="LoginRequest" />.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the getloginsessionrequest.
        /// </summary>
        [JsonProperty("getloginsessionrequest")]
        public Getloginsessionrequest getloginsessionrequest { get; set; }
    }
}
