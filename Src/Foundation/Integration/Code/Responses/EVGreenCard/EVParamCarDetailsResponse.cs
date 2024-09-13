using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.EVGreenCard.RTAPARAM
{
    

    public class Return
    {
        public string description { get; set; }
        public string carRegistratedCountry { get; set; }
        public string carRegistratedRegion { get; set; }
        public string carplatenumber { get; set; }
        public string customercategory { get; set; }
        public string trafficFileNumber { get; set; }
        public string responsecode { get; set; }
    }

    public class EVParamCarDetailsResponse
    {
        [JsonProperty("return")]
        public Return @return { get; set; }
    }

    public class Body
    {
        public EVParamCarDetailsResponse GetEVParamCarDetailsResponse { get; set; }
    }

    public class Envelope
    {
        public Body Body { get; set; }
    }

    public class EVParamResponse
    {
        public Envelope Envelope { get; set; }
    }


}
