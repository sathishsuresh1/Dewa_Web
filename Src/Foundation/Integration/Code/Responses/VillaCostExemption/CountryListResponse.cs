using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.VillaCostExemption
{
    public class CountryListResponse
    {
        [JsonProperty("countrylist")]
        public List<CountryList> countryList { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
    }
    public class CountryList
    {
        [JsonProperty("countrycode")]
        public string countrycode { get; set; }
        [JsonProperty("countryid")]
        public string countryid { get; set; }
        [JsonProperty("countrydescription")]
        public string countrydescription { get; set; }
        
    }
}
