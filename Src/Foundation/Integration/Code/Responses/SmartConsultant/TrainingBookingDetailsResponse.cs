using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class TrainingBookingDetailsResponse
    {
        [JsonProperty("applicantname")]
        public string applicantname { get; set; }
        [JsonProperty("certificate")]
        public string certificate { get; set; }
        [JsonProperty("certificatenumber")]
        public string certificatenumber { get; set; }
        [JsonProperty("companycontactperson")]
        public string companycontactperson { get; set; }
        [JsonProperty("companydescription")]
        public string companydescription { get; set; }
        [JsonProperty("companyemail")]
        public string companyemail { get; set; }
        [JsonProperty("companymobile")]
        public string companymobile { get; set; }
        [JsonProperty("companyname")]
        public string companyname { get; set; }
        [JsonProperty("countryname")]
        public string countryname { get; set; }
        [JsonProperty("departmenttext")]
        public string departmenttext { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("designation")]
        public string designation { get; set; }
        [JsonProperty("designesexp")]
        public string designesexp { get; set; }
        [JsonProperty("designpvexp")]
        public string designpvexp { get; set; }
        [JsonProperty("emailaddress")]
        public string emailaddress { get; set; }
        [JsonProperty("emiratesid")]
        public string emiratesid { get; set; }
        [JsonProperty("enddate")]
        public string enddate { get; set; }
        [JsonProperty("licenseexpirydate")]
        public string licenseexpirydate { get; set; }
        [JsonProperty("licenseissuedate")]
        public string licenseissuedate { get; set; }
        [JsonProperty("mobilenumber")]
        public string mobilenumber { get; set; }
        [JsonProperty("passportexpirydate")]
        public string passportexpirydate { get; set; }
        [JsonProperty("passportissuedate")]
        public string passportissuedate { get; set; }
        [JsonProperty("passportnumber")]
        public string passportnumber { get; set; }
        [JsonProperty("reasonforenroll")]
        public string reasonforenroll { get; set; }
        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
        [JsonProperty("shamsexp")]
        public string shamsexp { get; set; }
        [JsonProperty("startdate")]
        public string startdate { get; set; }
        [JsonProperty("tradelicense")]
        public string tradelicense { get; set; }
        [JsonProperty("trainingduration")]
        public string trainingduration { get; set; }
        [JsonProperty("trainingid")]
        public string trainingid { get; set; }
        [JsonProperty("trainingname")]
        public string trainingname { get; set; }
        [JsonProperty("vatnumber")]
        public string vatnumber { get; set; }
        [JsonProperty("visaissuedate")]
        public string visaissuedate { get; set; }
        [JsonProperty("visanumber")]
        public string visanumber { get; set; }
        [JsonProperty("visavaliditydate")]
        public string visavaliditydate { get; set; }
        [JsonProperty("applicationstatus")]
        public string applicationstatus { get; set; }
    }
}
