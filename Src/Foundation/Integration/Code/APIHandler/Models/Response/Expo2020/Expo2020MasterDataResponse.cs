using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Expo2020
{
    [Serializable]
    public class Expo2020MasterDataResponse : ApiBaseResponse
    {
       [JsonProperty("details")]
        public List<MasterDataDetails> details { get; set; }
    }
    public class MasterDataDetails
    {
        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("majordeveloper")]
        public string majordeveloper { get; set; }
    }
}

