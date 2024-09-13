using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests
{
    [Serializable]
    public class PVCCCertificateDetailsRequest
    {
        [JsonProperty("requestnumber")]
        public string requestnumber;

        [JsonProperty("certificatenumber")]
        public string certificatenumber;

        [JsonProperty("emiratesid")]
        public string emiratesid;

        [JsonProperty("appversion")]
        public string appversion;

        [JsonProperty("mobileosversion")]
        public string mobileosversion;

        [JsonProperty("appidentifier")]
        public string appidentifier;

        [JsonProperty("vendorid")]
        public string vendorid;
        //[JsonProperty("lang")]
        //public string lang;
    }
}
