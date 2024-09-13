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
    public class RammasGateWayResponse
    {
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public string ConversationId { get; set; }
    }
}