using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests.Tayseer
{
    /// <summary>
    /// Tayseer Detail Request
    /// </summary>
    public class TayseerDetailsRequest
    {
        public Referencenumberinputs referencenumberinputs { get; set; }
    }
    /// <summary>
    /// Reference Number Inputs
    /// </summary>
    public class Referencenumberinputs
    {
        public string appidentifier { get; set; }
        public string appversion { get; set; }
        public string mobileosversion { get; set; }
        public string dewareferencenumber { get; set; }
        public string projectdescription { get; set; }
        public string sessionid { get; set; }
        public string userid { get; set; }
        public string vendorid { get; set; }
        public string lang { get; set; }
        public string verifyflag { get; set; }
        public string emailid { get; set; }
        public string mobilenumber { get; set; }
        public List<Referenceaccountlist> referenceaccountlist { get; set; }
    }
    /// <summary>
    /// Refernce Account List
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
