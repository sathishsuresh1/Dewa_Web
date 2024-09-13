// <copyright file="UpdateConsultantTrainings.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Mayur Prajapati</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartConsultant
{
    using Newtonsoft.Json;

    public class UpdateConsultantTrainingAttachResponse
    {
        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
        [JsonProperty("reqnumber")]
        public string reqnumber { get; set; }
    }
}
