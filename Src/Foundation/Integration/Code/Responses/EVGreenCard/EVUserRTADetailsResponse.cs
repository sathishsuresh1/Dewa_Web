using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Integration.Responses.EVGreenCard.RTA
{
    public class EVUserRTADetailsResponse
    {
        public Envelope Envelope { get; set; }
    }

    public class Return
    {
        public object bpNumber { get; set; }
        public string description { get; set; }
        public string emailId { get; set; }
        public string firstName { get; set; }
        public string idNumber { get; set; }
        public string idType { get; set; }
        public object lastName { get; set; }
        public string mobileNumber { get; set; }
        public object nationaliy { get; set; }
        public object poBox { get; set; }
        public object region { get; set; }
        public object requestNumber { get; set; }
        public string responseCode { get; set; }
        public object userId { get; set; }
    }

    public class SetNewEVGreenCardV2Response
    {
        public Return @return { get; set; }
    }

    public class Body
    {
        public SetNewEVGreenCardV2Response SetNewEVGreenCardV2Response { get; set; }
    }

    public class Envelope
    {
        public Body Body { get; set; }
    }

   
}