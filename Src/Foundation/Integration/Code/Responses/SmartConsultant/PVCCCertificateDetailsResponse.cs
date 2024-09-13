using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses
{
    [Serializable]
    public class PVCCCertificateDetailsResponse
    {
        [JsonProperty("applicantname")]
        public string applicantname { get; set; }
        [JsonProperty("companyname")]
        public string companyname { get; set; }
        [JsonProperty("certificatecategory")]
        public string certificatecategory { get; set; }
        [JsonProperty("certificatecontent")]
        public string certificatecontent { get; set; }
        [JsonProperty("certificateexpirydate")]
        public string certificateexpirydate { get; set; }
        [JsonProperty("certificateissued")]
        public string certificateissued { get; set; }
        [JsonProperty("certificateissuedate")]
        public string certificateissuedate { get; set; }
        [JsonProperty("certificatenumber")]
        public string certificatenumber { get; set; }
        [JsonProperty("certificatestatus")]
        public string certificatestatus { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("emiratesid")]
        public string emiratesid { get; set; }
        [JsonProperty("enddate")]
        public string enddate { get; set; }
        [JsonProperty("requestnumber")]
        public string requestnumber { get; set; }
        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
        [JsonProperty("startdate")]
        public string startdate { get; set; }
        [JsonProperty("trainingid")]
        public string trainingid { get; set; }
    }
}
