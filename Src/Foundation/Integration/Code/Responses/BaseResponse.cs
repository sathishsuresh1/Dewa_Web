using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class BaseResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }

    public class RestBaseResponse
    {
        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("responsecode")]
        public string responsecode { get; set; }

        [JsonProperty("requestNumber")]
        public string requestNumber { get; set; }

        [JsonProperty("notificationNumber")]
        public string notificationNumber { get; set; }
    }

   
}
