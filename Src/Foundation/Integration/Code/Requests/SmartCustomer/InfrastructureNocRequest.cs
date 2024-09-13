using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer
{
    [Serializable]
    public class InfrastructureNocRequest
    {
        [JsonProperty("sessionid")]
        public string sessionid;

        [JsonProperty("userid")]
        public string userid;

        [JsonProperty("fileid")]
        public string fileid;

        [JsonProperty("transactionid")]
        public string transactionid;

        [JsonProperty("businesspartner")]
        public string businesspartner;

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
