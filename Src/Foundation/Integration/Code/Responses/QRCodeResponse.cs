using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Integration.Responses
{

    [Serializable]
	public class QRCodeResponse
    {
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("logmessagenumber")]
        public string logmessagenumber { get; set; }
        [JsonProperty("lognumber")]
        public string lognumber { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("message1")]
        public string message1 { get; set; }
        [JsonProperty("message2")]
        public string message2 { get; set; }
        [JsonProperty("message3")]
        public string message3 { get; set; }
        [JsonProperty("message4")]
        public string message4 { get; set; }
        [JsonProperty("number")]
        public string number { get; set; }
        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
        [JsonProperty("employeedata")]
        public recordsresponse employeedata { get; set; }
    }
    public class recordsresponse
    {
        [JsonProperty("records")]
        public List<recordsdata> records { get; set; }
    }
    public class recordsdata
    {
        [JsonProperty("label")]
        public string label { get; set; }
        [JsonProperty("value")]
        public string value { get; set; }
    }
}
