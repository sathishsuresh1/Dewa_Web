// <copyright file="InfrastructureNocSubmitReponse.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Hansraj Rathva</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartCustomer
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class InfrastructureNocSubmitReponse
    {
        [JsonProperty("DEWAnum")]
        public string DEWAnum { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }
  
        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
    }
}
