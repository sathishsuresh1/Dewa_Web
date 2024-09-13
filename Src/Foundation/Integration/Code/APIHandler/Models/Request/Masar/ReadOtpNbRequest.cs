using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class ReadOtpNbRequest
    {
        public ReadotpinputsEID readotpinputs { get; set; }
    }

    public class ReadotpinputsEID
    {
        public string processtype { get; set; }
        public string inputid { get; set; }
        public string idtype { get; set; }
        public string otpmode { get; set; }
    }
}
