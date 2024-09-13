using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword
{
    public class OTPResponse
    {
        //public Return @return { get; set; }
        public string description { get; set; }
        public List<Emaillist> emaillist { get; set; }
        public string maxattempts { get; set; }
        public List<Mobilelist> mobilelist { get; set; }
        public string newcredential { get; set; }
        public string responsecode { get; set; }
        public string validityminutes { get; set; }
        public string validityseconds { get; set; }
    }

    public class Emaillist
    {
        public string maskedemail { get; set; }
        public string unmaskedemail { get; set; }
    }

    public class Mobilelist
    {
        public string maskedmobile { get; set; }
        public string unmaskedmobile { get; set; }
    }

    public class Return
    {
        
    }
}
