// <copyright file="InfrastructureNocStatusResponse.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Hansraj Rathva</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartCustomer
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class InfrastructureNocStatusResponse
    {
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("interactionhistory")]
        public string interactionhistory { get; set; }
        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
        [JsonProperty("status")]
        public List<Status> status { get; set; }
    }
    public class Status
    {
        [JsonProperty("color")]
        public string color { get; set; }
        [JsonProperty("colorcode")]
        public string colorcode { get; set; }
        [JsonProperty("date")]
        public string date { get; set; }
        [JsonProperty("datedescription")]
        public string datedescription { get; set; }
        [JsonProperty("statusdescription")]
        public string statusdescription { get; set; }
        [JsonProperty("userstatus")]
        public string userstatus { get; set; }
    }
}
