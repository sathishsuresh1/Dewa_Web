using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace DEWAXP.Foundation.Content.Models.RammasLogin
{
    public class RammasGateWayRequest
    {
        /// <summary>
        /// Gets or sets the Contract Accounts.
        /// </summary>
        public string ContractAccounts;

        /// <summary>
        /// Gets or sets Amount.
        /// </summary>
        public string Amount;

        /// <summary>
        /// Gets or sets UserId.
        /// </summary>
        public string UserId;

        /// <summary>
        ///     Gets or sets the channel id.
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        ///     Gets or sets the service url.
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        ///     Gets or sets the conversation id.
        /// </summary>
        public string ConversationId { get; set; }
        public string TransactionType { get; set; }
        public string FromId { get; set; }
        public string FromName { get; set; }
        public string ToId { get; set; }
        public string ToName { get; set; }
        public string LanguageCode { get; set; }
        public string ActivityId { get; set; }
        public string Type { get; set; }
        public string D { get; set; }
        public string EPayTransactionCode { get; set; }
        public bool ThirdPartyPayment { get; set; }
        public string BusinessPartnerNumber { get; set; }
        public string OwnerPartnerNumber { get; set; }
        public string EMailId { get; set; }
        public string MobileNumber { get; set; }
        public string ConsultantpartnerNumber { get; set; }
        public string EasyPayNumber { get; set; }
        public string SessionId { get; set; }
        public string SuqiaValue { get; set; }
        public string Suqiaamt { get; set; }
    }
}