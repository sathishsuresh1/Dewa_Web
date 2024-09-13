using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarBankRequest
    {
        public MasarBankRequest() { bankdisplayreq = new Bankdisplayreq(); }
        public Bankdisplayreq bankdisplayreq { get; set; }
    }

    public class Bankdisplayreq
    {
        public string lang { get; set; }
        public string userid { get; set; }
        public string sessionid { get; set; }
        public string requestnumber { get; set; }
    }
}
