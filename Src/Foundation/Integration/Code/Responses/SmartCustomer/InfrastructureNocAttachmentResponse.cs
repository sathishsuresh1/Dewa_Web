// <copyright file="InfrastructureNocAttachmentResponse.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Hansraj Rathva</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartCustomer
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class InfrastructureNocAttachmentResponse
    {
        [JsonProperty("attach")]
        public attachment attach { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
    }
}
