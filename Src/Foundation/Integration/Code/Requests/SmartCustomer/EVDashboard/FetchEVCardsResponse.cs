// <copyright file="FetchEVCardsResponse.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Evcarddetail" />.
    /// </summary>
    public class Evcarddetail
    {
        /// <summary>
        /// Defines the bookmarkflag.
        /// </summary>
        [JsonProperty("bookmarkflag")]
        public string bookmarkflag;

        /// <summary>
        /// Defines the contractaccountname.
        /// </summary>
        [JsonProperty("contractaccountname")]
        public string contractaccountname;

        /// <summary>
        /// Defines the cardnumber.
        /// </summary>
        [JsonProperty("cardnumber")]
        public string cardnumber;

        /// <summary>
        /// Defines the contractaccount.
        /// </summary>
        [JsonProperty("contractaccount")]
        public string contractaccount;

        /// <summary>
        /// Defines the nickname.
        /// </summary>
        [JsonProperty("nickname")]
        public string nickname;

        /// <summary>
        /// Defines the platenumber.
        /// </summary>
        [JsonProperty("platenumber")]
        public string platenumber;

        /// <summary>
        /// Defines the rfid.
        /// </summary>
        [JsonProperty("rfid")]
        public string rfid;

        /// <summary>
        /// Defines the serialnumber.
        /// </summary>
        [JsonProperty("serialnumber")]
        public string serialnumber;

        /// <summary>
        /// Defines the username.
        /// </summary>
        [JsonProperty("username")]
        public string username;

        /// <summary>
        /// Defines the activeflag.
        /// </summary>
        [JsonProperty("activeflag")]
        public string activeflag;

        /// <summary>
        /// Defines the cardactivationdate.
        /// </summary>
        [JsonProperty("cardactivationdate")]
        public string cardactivationdate;

        /// <summary>
        /// Defines the deactivationdate.
        /// </summary>
        [JsonProperty("deactivationdate")]
        public string deactivationdate;
    }

    /// <summary>
    /// Defines the <see cref="FetchEVCardsResponse" />.
    /// </summary>
    public class FetchEVCardsResponse
    {
        /// <summary>
        /// Defines the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description;

        /// <summary>
        /// Defines the Evcarddetails.
        /// </summary>
        [JsonProperty("evcarddetails")]
        public List<Evcarddetail> Evcarddetails;

        /// <summary>
        /// Defines the Responsecode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string Responsecode;
    }
}
