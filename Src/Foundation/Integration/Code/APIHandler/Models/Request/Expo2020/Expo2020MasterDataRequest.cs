using Newtonsoft.Json;
using System;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Expo2020
{
    [Serializable]
    public class Expo2020MasterDataRequest
    {
        [JsonProperty("appidentifier")]
        public string appidentifier;

        [JsonProperty("appversion")]
        public string appversion;

        [JsonProperty("lang")]
        public string lang;

        [JsonProperty("mobileosversion")]
        public string mobileosversion;

        [JsonProperty("vendorid")]
        public string vendorid;
    }
}
