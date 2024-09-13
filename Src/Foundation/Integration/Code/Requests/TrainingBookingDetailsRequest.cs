using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests
{
    public class TrainingBookingDetailsRequest
    {
        [JsonProperty("requestnumber")]
        public string requestnumber;

        [JsonProperty("emiratesid")]
        public string emiratesid;

        [JsonProperty("vendorid")]
        public string vendorid;
    }
}
