using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarFetchICARequest
    {
        public Icainputs icainputs { get; set; }
    }

    public class Icainputs
    {
        public string emiratesid { get; set; }
        public string emiratesexpirydate { get; set; }
        public string lang { get; set; }
        public string userid { get; set; }
        public string sessionid { get; set; }
    }

    public class SendICARequest
    {
        public string emiratesid { get; set; }
        public string emiratesexpirydate { get; set; }
      
    }

}
