using Newtonsoft.Json;
using System;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Expo2020
{
    [Serializable]
    public class Expo2020Request
    {
        [JsonProperty("smartformsubmission")]
        public SmartFormSubmission smartformsubmission;
    }
    public class SmartFormSubmission
    {
        [JsonProperty("appidentifier")]
        public string appidentifier;

        [JsonProperty("appversion")]
        public string appversion;

        [JsonProperty("category")]
        public string category;

        [JsonProperty("contactperson")]
        public string contactperson;

        [JsonProperty("description")]
        public string description;

        [JsonProperty("emailid")]
        public string emailid;

        [JsonProperty("lang")]
        public string lang;

        [JsonProperty("majordeveloper")]
        public string majordeveloper;

        [JsonProperty("mobile")]
        public string mobile;

        [JsonProperty("mobileosversion")]
        public string mobileosversion;

        [JsonProperty("sessionid")]
        public string sessionid;

        [JsonProperty("text")]
        public string text;

        [JsonProperty("userid")]
        public string userid;

        [JsonProperty("vendorid")]
        public string vendorid;
    }

}
