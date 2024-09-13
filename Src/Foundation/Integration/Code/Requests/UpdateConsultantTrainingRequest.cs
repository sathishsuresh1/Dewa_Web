// <copyright file="UpdateConsultantTrainings.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Mayur Prajapati</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartConsultant
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class UpdateConsultantTrainingRequest
    {
        [JsonProperty("trainingdetails")]
        public TrainingDetails trainingdetails;
    }
    public class TrainingDetails
    {
        [JsonProperty("startdate")]
        public long? startdate;

        [JsonProperty("enddate")]
        public long? enddate;

        [JsonProperty("trainingid")]
        public long trainingid;

        [JsonProperty("emiratesid")]
        public long emiratesid;

        [JsonProperty("applicantname")]
        public string applicantname;

        [JsonProperty("trainingname")]
        public string trainingname;

        [JsonProperty("trainingduration")]
        public int trainingduration;

        [JsonProperty("certificatenumber")]
        public string certificatenumber;

        [JsonProperty("reasonforenroll")]
        public string reasonforenroll;

        [JsonProperty("countryname")]
        public string countryname;

        [JsonProperty("departmenttext")]
        public string departmenttext;

        [JsonProperty("designation")]
        public string designation;

        [JsonProperty("solarpvexpert")]
        public string solarpvexpert;

        [JsonProperty("visanumber")]
        public string visanumber;

        [JsonProperty("visaissuedate")]
        public long visaissuedate;

        [JsonProperty("visavaliditydate")]
        public long visavaliditydate;

        [JsonProperty("passportnumber")]
        public string passportnumber;

        [JsonProperty("passportissuedate")]
        public long passportissuedate;

        [JsonProperty("passportexpirydate")]
        public long passportexpirydate;

        [JsonProperty("companyname")]
        public string companyname;

        [JsonProperty("tradelicense")]
        public string tradelicense;

        [JsonProperty("licenseissuedate")]
        public long licenseissuedate;

        [JsonProperty("licenseexpirydate")]
        public long licenseexpirydate;

        [JsonProperty("vatnumber")]
        public string vatnumber;

        [JsonProperty("companydescription")]
        public string companydescription;

        [JsonProperty("companyemail")]
        public string companyemail;

        [JsonProperty("companymobile")]
        public long companymobile;

        [JsonProperty("companycontactperson")]
        public string companycontactperson;

        [JsonProperty("emailaddress")]
        public string emailaddress;

        [JsonProperty("mobilenumber")]
        public long mobilenumber;

        [JsonProperty("designesexp")]
        public string designesexp;

        [JsonProperty("designpvexp")]
        public string designpvexp;

        [JsonProperty("shamsexp")]
        public string shamsexp;

        [JsonProperty("additionalnote")]
        public string additionalnote;

        [JsonProperty("appver")]
        public string appver;

        [JsonProperty("mobileosver")]
        public string mobileosver;

        [JsonProperty("appidentifier")]
        public string appidentifier;

        [JsonProperty("lang")]
        public string lang;

        [JsonProperty("vendorid")]
        public string vendorid;
    }

}
