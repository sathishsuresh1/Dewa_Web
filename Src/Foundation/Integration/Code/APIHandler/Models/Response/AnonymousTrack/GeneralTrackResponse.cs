using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.AnonymousTrack
{
    [Serializable]
    public class GeneralTrackResponse
    {
        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("evcardno")]
        public string evcardno { get; set; }

        [JsonProperty("evnotification")]
        public string evnotification { get; set; }

        [JsonProperty("evstatus")]
        public string evstatus { get; set; }

        [JsonProperty("evstatusdescription")]
        public string evstatusdescription { get; set; }

        [JsonProperty("notificationnumber")]
        public string notificationnumber { get; set; }

        [JsonProperty("notificationtype")]
        public string notificationtype { get; set; }

        [JsonProperty("notificationtypetext")]
        public string notificationtypetext { get; set; }

        [JsonProperty("shorttext")]
        public string shorttext { get; set; }

        [JsonProperty("responsecode")]
        public string responsecode { get; set; }

        [JsonProperty("tracklist")]
        public List<TrackStatusList> tracklist { get; set; }
    }
    [Serializable]
    public class TrackStatusList
    {
        [JsonProperty("additionalfield1")]
        public string additionalfield1 { get; set; }

        [JsonProperty("additionalfield2")]
        public string additionalfield2 { get; set; }

        [JsonProperty("additionalfield3")]
        public string additionalfield3 { get; set; }

        [JsonProperty("additionalfield4")]
        public string additionalfield4 { get; set; }

        [JsonProperty("countryfullname")]
        public string countryfullname { get; set; }

        [JsonProperty("currentstatus")]
        public string currentstatus { get; set; }

        [JsonProperty("dateofnotification")]
        public string dateofnotification { get; set; }

        [JsonProperty("deletestatus")]
        public string deletestatus { get; set; }

        [JsonProperty("finalstatus")]
        public string finalstatus { get; set; }

        [JsonProperty("firstname")]
        public string firstname { get; set; }

        [JsonProperty("fromxgps")]
        public string fromxgps { get; set; }

        [JsonProperty("fromygps")]
        public string fromygps { get; set; }

        [JsonProperty("guid")]
        public string guid { get; set; }

        [JsonProperty("iban")]
        public string iban { get; set; }

        [JsonProperty("ibanmessage")]
        public string ibanmessage { get; set; }

        [JsonProperty("lastname")]
        public string lastname { get; set; }

        [JsonProperty("map")]
        public string map { get; set; }

        [JsonProperty("mtcnvalidto")]
        public string mtcnvalidto { get; set; }

        [JsonProperty("notificationdate")]
        public string notificationdate { get; set; }

        [JsonProperty("notificationnumber")]
        public string notificationnumber { get; set; }

        [JsonProperty("notificationstatus")]
        public string notificationstatus { get; set; }

        [JsonProperty("notificationtext")]
        public string notificationtext { get; set; }

        [JsonProperty("notificationtextstatus")]
        public string notificationtextstatus { get; set; }

        [JsonProperty("notificationtime")]
        public string notificationtime { get; set; }

        [JsonProperty("objectstatus")]
        public string objectstatus { get; set; }

        [JsonProperty("objectstatusdescription")]
        public string objectstatusdescription { get; set; }

        [JsonProperty("objectstatustext")]
        public string objectstatustext { get; set; }

        [JsonProperty("refundactstatus")]
        public string refundactstatus { get; set; }

        [JsonProperty("refundamount")]
        public string refundamount { get; set; }

        [JsonProperty("refundappstatus")]
        public string refundappstatus { get; set; }

        [JsonProperty("refundcode")]
        public string refundcode { get; set; }

        [JsonProperty("refundcomments")]
        public string refundcomments { get; set; }

        [JsonProperty("refundcurrency")]
        public string refundcurrency { get; set; }

        [JsonProperty("refundprocessstatus")]
        public string refundprocessstatus { get; set; }

        [JsonProperty("refundseccode")]
        public string refundseccode { get; set; }

        [JsonProperty("refundstatuscode")]
        public string refundstatuscode { get; set; }

        [JsonProperty("refundstatusdescription")]
        public string refundstatusdescription { get; set; }

        [JsonProperty("refundtransid")]
        public string refundtransid { get; set; }

        [JsonProperty("refundtype")]
        public string refundtype { get; set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; set; }

        [JsonProperty("toxgps")]
        public string toxgps { get; set; }

        [JsonProperty("toygps")]
        public string toygps { get; set; }
    }
}
