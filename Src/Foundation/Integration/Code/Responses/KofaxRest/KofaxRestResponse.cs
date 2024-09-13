using Newtonsoft.Json;

namespace DEWAXP.Foundation.Integration.Responses.KofaxRest
{
    public partial class KofaxRestResponse
    {
        [JsonProperty("executionTime")]
        public long ExecutionTime { get; set; }

        [JsonProperty("robot-error")]
        public RobotError RobotError { get; set; }

        [JsonProperty("values")]
        public Value[] Values { get; set; }
    }

    public partial class RobotError
    {
        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("robotUrl")]
        public string RobotUrl { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("errorLocation")]
        public string ErrorLocation { get; set; }

        [JsonProperty("errorLocationCode")]
        public string ErrorLocationCode { get; set; }
    }
    public partial class Value
    {
        [JsonProperty("typeName")]
        public string TypeName { get; set; }

        [JsonProperty("attribute")]
        public Attribute[] Attribute { get; set; }
    }
    public partial class Attribute
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
