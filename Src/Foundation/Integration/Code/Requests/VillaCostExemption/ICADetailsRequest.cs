using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests.VillaCostExemption
{
    public class ICADetailsRequest : BaseRequest
    {
        public string unifiednumber { get; set; }
        public string marsoomflag { get; set; }
        public eiddetails eiddetails { get; set; }
        public passportdetails passportdetails { get; set; }
    }
    public class eiddetails
    {
        public string dateofbirth { get; set; }
        public string gender { get; set; }
        public string idnumber { get; set; }
    }
    public class passportdetails
    {
        public string dateofbirth { get; set; }
        public string documentnumber { get; set; }
        public string documenttype { get; set; }
        public string gender { get; set; }
        public string nationality { get; set; }
    }
}
