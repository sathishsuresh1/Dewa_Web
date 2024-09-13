using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.DTMCInsightsReport
{
    /// <summary>
    /// GetWaterAI Response
    /// </summary>
    public class GetWaterAIResponse
    {
        [JsonProperty("return")]
        public GetWaterAI_reponsedata ReturnData { get; set; }
        [JsonProperty("responsecode")]
        public string ResponseCode { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class GetWatereAI_Alarm
    {
        public string date { get; set; }
        public string type { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
    }

    public class GetWatereAI_dailyconsumption
    {
        public string date { get; set; }
        public string leak { get; set; }
        public string status { get; set; }
    }
    public class GetWatereAI_Agg
    {
        public string midday { get; set; }
        public string night { get; set; }
        public string consumption { get; set; }
        public string evening { get; set; }
        public string morning { get; set; }
    }

    public class GetWaterAI_consolidated
    {
        public int cotcode { get; set; }
        public string reason { get; set; }
        public int reasoncode { get; set; }
        public string cot { get; set; }
    }

    public class GetWaterAI_reponsedata
    {
        [JsonProperty("consolidated")]
        public List<GetWaterAI_consolidated> Consolidated { get; set; }
        [JsonProperty("aggs")]
        public GetWatereAI_Agg Aggs { get; set; }
        [JsonProperty("alarms")]
        public List<GetWatereAI_Alarm> Alarms { get; set; }
        [JsonProperty("daily")]
        public List<GetWatereAI_dailyconsumption> daily { get; set; }
    }
}
