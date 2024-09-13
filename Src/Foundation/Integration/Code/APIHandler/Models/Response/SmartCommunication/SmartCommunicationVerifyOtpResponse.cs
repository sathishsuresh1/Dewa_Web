using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.SmartCommunication
{
    public class SmartCommunicationVerifyOtpResponse
    {

        public string description { get; set; }
        public List<SmartCommunicationEmailResponse> emaillist { get; set; }
        public List<SmartCommunicationMobileResponse> mobilelist { get; set; }
        public string responseCode { get; set; }
    }

    public class SmartCommunicationEmailResponse
    {
        public string maskedemail { get; set; }
        public string unmaskedemail { get; set; }
    }

    public class SmartCommunicationMobileResponse
    {
        public string maskedmobile { get; set; }
        public string unmaskedmobile { get; set; }
    }
}

