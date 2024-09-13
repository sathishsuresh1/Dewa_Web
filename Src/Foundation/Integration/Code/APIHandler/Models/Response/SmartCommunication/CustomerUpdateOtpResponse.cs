using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.SmartCommunication
{
    public class CustomerUpdateOtpResponse
    {
        public string businesspartnernumber { get; set; }
        public string description { get; set; }
        public string flag { get; set; }
        public string maxattempts { get; set; }
        public string newcredential { get; set; }
        public string otprequestid { get; set; }
        public string responsecode { get; set; }
        public string validityminutes { get; set; }
        public string validityseconds { get; set; }
    }
}
