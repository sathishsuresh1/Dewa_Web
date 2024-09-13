// <copyright file="UpdateConsultantTrainings.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Mayur Prajapati</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartConsultant
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class UpdateConsultantTrainingAttachRequest
    {
        [JsonProperty("trainingattachmentDetails")]
        public List<TrainingAttachmentDetails> trainingattachmentDetails;

        [JsonProperty("appversion")]
        public string appversion;

        [JsonProperty("mobileosversion")]
        public string mobileosversion;

        [JsonProperty("appidentifier")]
        public string appidentifier;

        [JsonProperty("lang")]
        public string lang;

        [JsonProperty("flag")]
        public string flag;

        [JsonProperty("vendorid")]
        public string vendorid;
    }
    public class TrainingAttachmentDetails
    {
        [JsonProperty("documenttype")]
        public string documenttype;

        [JsonProperty("emiratesid")]
        public long emiratesid;

        [JsonProperty("filecontent")]
        public string filecontent;

        [JsonProperty("filename")]
        public string filename;

        [JsonProperty("filesize")]
        public long filesize;

        [JsonProperty("mimetype")]
        public string mimetype;

        [JsonProperty("trainingid")]
        public long trainingid;

        [JsonProperty("reqnumber")]
        public string reqnumber;
    }

}
