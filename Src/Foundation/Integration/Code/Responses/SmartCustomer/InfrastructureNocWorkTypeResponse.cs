// <copyright file="InfrastructureNocWorkTypeResponse.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\Hansraj Rathva</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartCustomer
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class InfrastructureNocWorkTypeResponse
    {
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("POWorkDescription")]
        public List<WorkTypeDetails> WorkTypeList { get; set; }
        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
    }
    public class WorkTypeDetails
    {
        [JsonProperty("descproposedWorktype")]
        public string DescProposedWorkType { get; set; }
        [JsonProperty("proposedWorktype")]
        public string ProposedWorkType { get; set; }
    }
}
