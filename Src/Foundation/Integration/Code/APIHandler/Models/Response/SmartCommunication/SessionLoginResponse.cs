using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.SmartCommunication
{
    public class SessionLoginResponse
    {
        public  string businesspartnernumber { get; set; }
        public string fullname { get; set; }
        public string description { get; set; }
        public string responsecode { get; set; }
        public string sessionid { get; set; }
        public string maxattempt { get; set; }
    }
}
