using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarFetchDEDRequest
    {
        public Tradelicenseinput tradelicenseinput { get; set; }
    }

    public class Tradelicenseinput
    {
        public string tradelicensenumber { get; set; }
        public string lang { get; set; }
        public string userid { get; set; }
        public string sessionid { get; set; }
        public string issuingauthority { get; set; }
        public string issuedate { get; set; }
        public string expirydate { get; set; }       
        public string appversion { get; set; }
        public string appidentifier { get; set; }        
        public string mobileosversion { get; set; }
        public string type { get; set; }
        public string mode { get; set; }
    }

    public class SendDEDRequest
    {
        public string tradelicensenumber { get; set; }
        public string tlexpirydate { get; set; }
        public string issuingauthority { get; set; }
        public string mode { get; set; }
        public string type { get;set; }

    }

}
