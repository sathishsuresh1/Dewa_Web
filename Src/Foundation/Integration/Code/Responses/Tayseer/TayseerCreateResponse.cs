using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.Tayseer
{
    /// <summary>
    /// Tayseer Create Response
    /// </summary>
    public class TayseerCreateResponse
    {
        public string chequepaymentallowed { get; set; }
        public string description { get; set; }
        public string dewareferencenumber { get; set; }
        public List<Referenceaccountlist> referenceaccountlist { get; set; }
        public string responsecode { get; set; }
        public string totalaccounts { get; set; }
        public string totalamount { get; set; }
    }
    /// <summary>
    /// Reference Account List
    /// </summary>
    public class Referenceaccountlist
    {
        public string amount { get; set; }
        public string businesspartner { get; set; }
        public string contractaccount { get; set; }
        public string finalbillflag { get; set; }
        public string paidamount { get; set; }
        public string remarks { get; set; }
        public string remarkscode { get; set; }
    }
}
