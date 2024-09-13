// <copyright file="UpdateConsultantTrainings.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Mayur Prajapati</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartConsultant
{
    using Newtonsoft.Json;

    public class PVCCEmiratesidVerifyResponse
    {
        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
    }
}
