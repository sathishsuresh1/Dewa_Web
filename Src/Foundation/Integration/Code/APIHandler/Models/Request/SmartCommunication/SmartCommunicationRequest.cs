using Newtonsoft.Json;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartCommunication
{
    public class SmartCommunicationRequest
    {
        [JsonProperty("userid")]
        public string userid { get; set; }

        [JsonProperty("premisenumber")]
        public string premisenumber { get; set; }

        [JsonProperty("contractaccountnumber")]
        public string contractaccountnumber { get; set; }

        [JsonProperty("businesspartnernumber")]
        public string businesspartnernumber { get; set; }

        [JsonProperty("requesttype")]
        public string requesttype { get; set; }

        [JsonProperty("mobilenumber")]
        public string mobilenumber { get; set; }

        [JsonProperty("remarks")]
        public string remarks { get; set; }

        [JsonProperty("lang")]
        public string lang { get; set; }
    }
}
