using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartCommunication
{
    [Serializable]
    public class SessionLoginRequest
    {
        [JsonProperty("sessionparams")]
        public SessionParams sessionparams { get; set; }
    }
    [Serializable]
    public class SessionParams
    {
        [JsonProperty("center")]
        public string center { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("lang")]
        public string lang { get; set; }
        [JsonProperty("logintype")]
        public string logintype { get; set; }
        [JsonProperty("merchantid")]
        public string merchantid { get; set; }
        [JsonProperty("merchantpassword")]
        public string merchantpassword { get; set; }
        [JsonProperty("mobile")]
        public string mobile { get; set; }
        [JsonProperty("otpkey")]
        public string otpkey { get; set; }
        [JsonProperty("referencenumber")]
        public string referencenumber { get; set; }
        [JsonProperty("userid")]
        public string userid { get; set; }
        [JsonProperty("xcoordinate")]
        public string xcoordinate { get; set; }
        [JsonProperty("ycoordinate")]
        public string ycoordinate { get; set; }
    }
}
