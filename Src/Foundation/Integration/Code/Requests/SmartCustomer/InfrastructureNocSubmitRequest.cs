using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer
{
    public class InfraNocSubmitRequest
    {
        [JsonProperty("InfraNocRequest")]
        public InfraNocRequest InfraNocRequest { get; set; }
    }
    public class InfraNocRequest
    {
        [JsonProperty("sessionid")]
        public string sessionid { get; set; }
        [JsonProperty("userid")]
        public string userid { get; set; }
        [JsonProperty("contractaccount")]
        public string contractaccount { get; set; }
        [JsonProperty("proposedWorktype")]
        public string proposedWorktype { get; set; }
        [JsonProperty("descproposedWorktype")]
        public string descproposedWorktype { get; set; }
        [JsonProperty("plotnumber")]
        public string plotnumber { get; set; }
        [JsonProperty("businesspartner")]
        public string businesspartner { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("transactionid")]
        public string transactionid { get; set; }
        [JsonProperty("statusdescription")]
        public string statusdescription { get; set; }
        [JsonProperty("submissiondate")]
        public string submissiondate { get; set; }
        [JsonProperty("customernotes")]
        public string customernotes { get; set; }
        [JsonProperty("interactionhistory")]
        public string interactionhistory { get; set; }
        [JsonProperty("appversion")]
        public string appversion { get; set; }
        [JsonProperty("mobileosversion")]
        public string mobileosversion { get; set; }
        [JsonProperty("appidentifier")]
        public string appidentifier { get; set; }
        [JsonProperty("vendorid")]
        public string vendorid { get; set; }

        [JsonProperty("lang")]
        public string lang;

        [JsonProperty("attach")]
        public Attach[] attach { get; set; }
    }
    public class Attach
    {
        [JsonProperty("filename")]
        public string filename;

        [JsonProperty("mimetype")]
        public string mimetype;

        [JsonProperty("fileid")]
        public string fileid;

        [JsonProperty("filesize")]
        public string filesize;

        [JsonProperty("filecontent")]
        public string filecontent;

        [JsonProperty("doctype")]
        public string doctype;

        [JsonProperty("folder")]
        public string folder;
    }
}
