using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class ApplyEVCardResponse : RestBaseResponse
    {
        public greenCardResponse greenCardApplication { get; set; }
    }

    public class greenCardResponse
    {
        [JsonProperty("bpFirstName")]
        public string bpFirstName { get; set; }

        [JsonProperty("bpLastName")]
        public string bpLastName { get; set; }

        [JsonProperty("bpNumber")]
        public string bpNumber { get; set; }

        [JsonProperty("emailId")]
        public string emailId { get; set; }

        [JsonProperty("mobileNumber")]
        public string mobileNumber { get; set; }

        [JsonProperty("requestNumber")]
        public string requestNumber { get; set; }

        [JsonProperty("userId")]
        public string userId { get; set; }
    }

    public class ReplaceEVCard : RestBaseResponse
    {
        public string cardFee { get; set; }
        public string courierFee { get; set; }
        public string taxAmount { get; set; }
        public string taxRate { get; set; }
        public string totalAmount { get; set; }
        public List<ReasonDetails> replaceReasonList { get; set; }
    }

    public class EVCardResponse : RestBaseResponse
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailId { get; set; }
        public string mobileNumber { get; set; }
        public string nationaliy { get; set; }
        public string poBox { get; set; }
        public string region { get; set; }
    }

    public class ReasonDetails
    {
        public string code { get; set; }

        public string description { get; set; }
    }
}
