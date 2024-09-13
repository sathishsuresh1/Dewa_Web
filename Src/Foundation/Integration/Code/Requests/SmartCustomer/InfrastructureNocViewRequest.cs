using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer
{
    [Serializable]
    public class InfrastructureNocViewRequest
    {
        [JsonProperty("sessionid")]
        public string SessionId;

        [JsonProperty("userid")]
        public string UserId;

        [JsonProperty("transactionid")]
        public string transactionid;

        [JsonProperty("appversion")]
        public string appversion;

        [JsonProperty("mobileosversion")]
        public string mobileosversion;

        [JsonProperty("appidentifier")]
        public string appidentifier;

        [JsonProperty("vendorid")]
        public string vendorid;

        [JsonProperty("lang")]
        public string lang;
    }
}
