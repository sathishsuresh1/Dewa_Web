using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack
{
    [Serializable]
    public class TrackListRequest
    {
        [JsonProperty("complaintsIn")]
        public ComplaintsIn complaintsIn { get; set; }
    }
    [Serializable]
    public class ComplaintsIn
    {
        [JsonProperty("contractaccountnumber")]
        public string contractaccountnumber { get; set; }
        [JsonProperty("businesspartner")]
        public string businesspartner { get; set; }
        [JsonProperty("mobilenumber")]
        public string mobilenumber { get; set; }
        [JsonProperty("lang")]
        public string lang { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("referencevalue")]
        public string referencevalue { get; set; }
        [JsonProperty("servicecode")]
        public string servicecode { get; set; }
    }
}
