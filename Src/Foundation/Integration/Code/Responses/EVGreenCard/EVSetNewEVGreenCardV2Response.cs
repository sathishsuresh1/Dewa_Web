using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.EVGreenCard.NCR
{
    public class Return
    {
        public string lastName { get; set; }
        public string idType { get; set; }
        public string mobileNumber { get; set; }
        public string description { get; set; }
        public string emailId { get; set; }
        public string idNumber { get; set; }
        public string userId { get; set; }
        public string responseCode { get; set; }
        public string firstName { get; set; }
        public string poBox { get; set; }
        public object requestNumber { get; set; }
        public object nationaliy { get; set; }
        public string region { get; set; }
        public object bpNumber { get; set; }
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

    public class NewCardResponse
    {
        public Envelope Envelope { get; set; }
    }
}
