using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarCancelBankRequest
    {
        public Bankdeletereq bankdeletereq { get; set; }
    }

    public class Bankdeletereq
    {
        public string lang { get; set; }
        public string userid { get; set; }
        public string sessionid { get; set; }
        public string requestnumber { get; set; }
    }

    public class VerifyIbanRequest
    {
        public string countrycode { get; set; }
        public string lang { get; set; }
    }

}
