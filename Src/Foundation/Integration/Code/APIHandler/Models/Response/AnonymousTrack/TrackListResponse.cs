using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.AnonymousTrack
{
    [Serializable]
    public class TrackListResponse
    {
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
        [JsonProperty("trackRequests")]
        public List<TrackRequests> trackRequests { get; set;  }
    }
    [Serializable]
    public class TrackRequests
    {
        [JsonProperty("attachFlag")]
        public string attachFlag { get; set; }
        [JsonProperty("businesspartner")]
        public string businesspartner { get; set; }
        [JsonProperty("codegroup")]
        public string codegroup { get; set; }
        [JsonProperty("codetext")]
        public string codetext { get; set; }
        [JsonProperty("complaintsIn")]
        public string complaintsIn { get; set; }
        [JsonProperty("completed")]
        public string completed { get; set; }
        [JsonProperty("completiondate")]
        public string completiondate { get; set; }
        [JsonProperty("completiontime")]
        public string completiontime { get; set; }
        [JsonProperty("currentdate")]
        public string currentdate { get; set; }
        [JsonProperty("dateofcreation")]
        public string dateofcreation { get; set; }
        [JsonProperty("equipment")]
        public string equipment { get; set; }
        [JsonProperty("generalflag")]
        public string generalflag { get; set; }
        [JsonProperty("groupode")]
        public string groupode { get; set; }
        [JsonProperty("lang")]
        public string lang { get; set; }
        [JsonProperty("newstatus")]
        public string newstatus { get; set; }
        [JsonProperty("notificationnumber")]
        public string notificationnumber { get; set; }
        [JsonProperty("notificationtype")]
        public string notificationtype { get; set; }
        [JsonProperty("notificationtypetext")]
        public string notificationtypetext { get; set; }
        [JsonProperty("objectpart")]
        public string objectpart { get; set; }
        [JsonProperty("shorttext")]
        public string shorttext { get; set; }
        [JsonProperty("startdate")]
        public string startdate { get; set; }
        [JsonProperty("starttime")]
        public string starttime { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("timeofnotification")]
        public string timeofnotification { get; set; }
    }
}
