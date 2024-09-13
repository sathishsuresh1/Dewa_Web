using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer
{
    public class slabTarrifRequest
    {
        public slabTarrifIn slabTarrifIn { get; set; }
    }
    public class  slabTarrifIn
    {
        /// <summary>
        /// Define response code
        /// </summary>
        [JsonProperty("contractAccount")]
        public string contractAccount { get; set; }
        /// <summary>
        /// Define response code
        /// </summary>
        [JsonProperty("credential")]
        public string credential { get; set; }
        /// <summary>
        /// Define response code
        /// </summary>
        [JsonProperty("lang")]

        public string lang { get; set; }
    }
}
