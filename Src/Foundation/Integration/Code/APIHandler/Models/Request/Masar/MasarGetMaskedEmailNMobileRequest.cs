using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarGetMaskedEmailNMobileRequest
    {
        public Readotpinputs readotpinputs { get; set; }
    }

    public class Readotpinputs
    {
        public string processtype { get; set; }
        public string inputid { get; set; }
        public string idtype { get; set; }
        public string otpmode { get; set; }
    }

    public class inputWrapperMaskedEmailNPhone
    {        
        public string userName { get; set; }
        public string dN { get; set; }
        public string referenceNumber { get; set; }
        public string actualOTP { get; set; }
    }

}
