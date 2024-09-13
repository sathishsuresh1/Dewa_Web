using Newtonsoft.Json;
using System;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Expo2020
{
    [Serializable]
    public class Expo2020Response : ApiBaseResponse
    {
        [JsonProperty("ordernumber")]
        public string ordernumber;

        [JsonProperty("subrc")]
        public string subrc;
    }
}

