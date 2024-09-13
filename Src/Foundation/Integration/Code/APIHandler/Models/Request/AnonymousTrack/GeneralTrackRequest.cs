using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack
{
    [Serializable]
    public class GeneralTrackRequest
    {
        [JsonProperty("trackrequestparams")]
        public TrackRequestParams trackrequestparams { get; set; }
    }
    [Serializable]
    public class TrackRequestParams
    {
        [JsonProperty("appidentifier")]
        public string appidentifier;

        [JsonProperty("appversion")]
        public string appversion;

        [JsonProperty("applicationflag")]
        public string applicationflag;

        [JsonProperty("lang")]
        public string lang;

        [JsonProperty("sessionid")]
        public string sessionid;

        [JsonProperty("userid")]
        public string userid;

        [JsonProperty("vendorid")]
        public string vendorid;

        [JsonProperty("logflag")]
        public string logflag;

        [JsonProperty("mobile")]
        public string mobile;

        [JsonProperty("email")]
        public string email;

        [JsonProperty("mobileosversion")]
        public string mobileosversion;

        [JsonProperty("notificationnumber")]
        public string notificationnumber;
    }
}
