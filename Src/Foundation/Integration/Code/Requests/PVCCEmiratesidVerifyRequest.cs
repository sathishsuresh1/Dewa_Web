using Newtonsoft.Json;
using System;

namespace DEWAXP.Foundation.Integration.Requests
{
    [Serializable]
    public class PVCCEmiratesidVerifyRequest
    {
        [JsonProperty("emiratesid")]
        public string emiratesid;

        [JsonProperty("OTP")]
        public string OTP;

        [JsonProperty("appversion")]
        public string appversion;

        [JsonProperty("mobileosversion")]
        public string mobileosversion;

        [JsonProperty("appidentifier")]
        public string appidentifier;

    }
}
