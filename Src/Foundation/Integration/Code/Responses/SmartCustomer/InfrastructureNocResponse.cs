// <copyright file="InfrastructureNocResponse.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Hansraj Rathva</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartCustomer
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class InfrastructureNocResponse
    {

        [JsonProperty("businesspartner")]
        public string businesspartner { get; set; }
        [JsonProperty("contractaccount")]
        public string contractaccount { get; set; }
        [JsonProperty("descproposedWorktype")]
        public string descproposedWorktype { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("plotnumber")]
        public string plotnumber { get; set; }
        [JsonProperty("proposedWorktype")]
        public string proposedWorktype { get; set; }
        [JsonProperty("revision")]
        public string revision { get; set; }
        [JsonProperty("developer")]
        public string developer { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("submissiondate")]
        public string submissiondate { get; set; }
        [JsonProperty("postingdate")]
        public string postingdate { get; set; }
        [JsonProperty("postingdescription")]
        public string postingdescription { get; set; }
        [JsonProperty("statusdescription")]
        public string statusdescription { get; set; }
        [JsonProperty("transactionid")]
        public string transactionid { get; set; }
        [JsonProperty("useralias")]
        public string useralias { get; set; }
        [JsonProperty("userstatus")]
        public string userstatus { get; set; }
        [JsonProperty("customernotes")]
        public string customernotes { get; set; }
        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
        [JsonProperty("uploadeddocuments")]
        public List<UploadedDocuments> uploadedDocuments { get; set; }
        [JsonProperty("dewadocuments")]
        public List<UploadedDocuments> dewadocuments { get; set; }
    }
    public class UploadedDocuments
    {
        [JsonProperty("attachment")]
        public List<attachment> AttachedDocuments { get; set; }
        [JsonProperty("folder")]
        public string folder { get; set; }
    }
    public class attachment
    {
        [JsonProperty("docdate")]
        public string docdate { get; set; }
        [JsonProperty("doctype")]
        public string doctype { get; set; }
        [JsonProperty("filecontent")]
        public string filecontent { get; set; }
        [JsonProperty("fileid")]
        public string fileid { get; set; }
        [JsonProperty("filename")]
        public string filename { get; set; }
        [JsonProperty("filesize")]
        public string filesize { get; set; }
        [JsonProperty("mimetype")]
        public string mimetype { get; set; }
        [JsonProperty("folder")]
        public string folder { get; set; }

    }
}
