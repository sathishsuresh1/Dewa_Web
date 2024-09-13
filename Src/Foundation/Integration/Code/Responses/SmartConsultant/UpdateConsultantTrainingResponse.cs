// <copyright file="UpdateConsultantTrainings.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Mayur Prajapati</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartConsultant
{
    using Newtonsoft.Json;

    public class UpdateConsultantTrainingResponse
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }
        [JsonProperty("requestnumber")]
        public string Requestnumber { get; set; }

    }
}
