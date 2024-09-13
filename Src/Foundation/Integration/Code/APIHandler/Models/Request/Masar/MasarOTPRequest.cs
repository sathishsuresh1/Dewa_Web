using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarOTPRequest
    {
        public MasarOtpInput otpinput { get; set; }
    }

    public class MasarOtpInput
    {
        public string email { get; set; }
        public string lang { get; set; }
        public string mobile { get; set; }
        public string mode { get; set; }
        public string otp { get; set; }
        public string prtype { get; set; }
        public string reference { get; set; }
        public string vendorid { get; set; }
        public string reqId { get; set; }
        public string sessionid { get; set; }
        public string inputid { get; set; }
        public string idtype { get; set; }
    }
}
