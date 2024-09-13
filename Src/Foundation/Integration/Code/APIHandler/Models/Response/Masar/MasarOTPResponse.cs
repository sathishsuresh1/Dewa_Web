using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar
{
    public class MasarOTPResponse
    {
        public string description { get; set; }
        public string email { get; set; }
        public string emailvalidity { get; set; }
        public string maxattempts { get; set; }
        public string mobile { get; set; }
        public string otp { get; set; }
        public string referencenumber { get; set; }
        public string responsecode { get; set; }
        public string smsvalidity { get; set; }
    }
}
