// <copyright file="LoginResponse.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Responses
{
    using DEWAXP.Foundation.Integration.Helpers;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="LoginResponse" />.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Defines the _accountNumber.
        /// </summary>
        private string _accountNumber;

        /// <summary>
        /// Defines the _businessPartner.
        /// </summary>
        private string _businessPartner;

        /// <summary>
        /// Gets a value indicating whether IsUpdateContact.
        /// </summary>
        public bool IsUpdateContact => Updatemobileemail;

        /// <summary>
        /// Gets or sets the BusinessPartner.
        /// </summary>
        [JsonProperty("businesspartner")]
        public string BusinessPartner
        {
            get => DewaResponseFormatter.Trimmer(_businessPartner);
            set => _businessPartner = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the Customertype.
        /// </summary>
        [JsonProperty("customertype")]
        public object Customertype { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the FullName.
        /// </summary>
        [JsonProperty("fullname")]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the Lastlogin.
        /// </summary>
        [JsonProperty("lastlogin")]
        public string Lastlogin { get; set; }

        /// <summary>
        /// Gets or sets the Mobile.
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PopupFlag.
        /// </summary>
        [JsonProperty("popup")]
        public bool PopupFlag { get; set; }

        /// <summary>
        /// Gets or sets the AccountNumber.
        /// </summary>
        [JsonProperty("primarycontractaccount")]
        public string AccountNumber
        {
            get => DewaResponseFormatter.Trimmer(_accountNumber);
            set => _accountNumber = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the ResponseCode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the SessionNumber.
        /// </summary>
        [JsonProperty("sessionid")]
        public string SessionNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AcceptedTerms.
        /// </summary>
        [JsonProperty("termsandcondition")]
        public bool AcceptedTerms { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Updatemobileemail.
        /// </summary>
        [JsonProperty("updatemobileemail")]
        public bool Updatemobileemail { get; set; }

        /// <summary>
        /// Gets or sets the UserCode.
        /// </summary>
        [JsonProperty("userCode")]
        public object UserCode { get; set; }

        /// <summary>
        /// Gets or sets the UserMessage.
        /// </summary>
        [JsonProperty("userMessage")]
        public object UserMessage { get; set; }

        /// <summary>
        /// Gets or sets the Userid.
        /// </summary>
        [JsonProperty("userid")]
        public string Userid { get; set; }
    }
}
