using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.JoinOwnership
{
    public class JointOwnerResponse
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("errorcode")]
        public string ErrorCode { get; set; }
        [JsonProperty("responsecode")]
        public string ResponseCode { get; set; }

        [JsonProperty("notificationNumber")]
        public string NotificationNumber { get; set; }

    }
}
