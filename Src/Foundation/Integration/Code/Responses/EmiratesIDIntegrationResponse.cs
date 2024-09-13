using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using SitecoreX = Sitecore.Context;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.Emirates
{

    public class EmiratesIDIntegrationRequest
    {
        public header header { get; set; }
        public eiddetails eiddetails { get; set; }
    }

    public class header
    {
        [JsonProperty("processid")]
        public string processid { get; set; }
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("password")]
        public string password { get; set; }
    }

    public class eiddetails
    {
        [JsonProperty("idnumber")]
        public string idnumber { get; set; }
        [JsonProperty("idexpirydate")]
        public string idexpirydate { get; set; }
    }

    public class EmiratesIDIntegrationResponse
    {
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("fullname")]
        public string fullname { get; set; }
        [JsonProperty("nationality")]
        public string nationality { get; set; }
        [JsonProperty("responsecode")]
        public string responsecode { get; set; }
        [JsonProperty("visaexpirydate")]
        public string visaexpirydate { get; set; }
        [JsonProperty("visanumber")]
        public string visanumber { get; set; }
        public string formatvisanumber
        {
            get
            {
                if (!string.IsNullOrEmpty(visanumber))
                {
                    return visanumber.Replace("/",string.Empty);
                }
                return visanumber;
            }
        }

        public string formatvisexpirydate
        {
            get
            {
                if (!string.IsNullOrEmpty(visaexpirydate))
                {
                    return DateTime.ParseExact(visaexpirydate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", SitecoreX.Culture);
                }
                return visanumber;
            }
        }
    }
}
