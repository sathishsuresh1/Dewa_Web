using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.Tayseer
{
    /// <summary>
    /// Tayseer List Response
    /// </summary>
    public class TayseerListResponse
    {
        public string description { get; set; }
        public List<Referencelist> referencelist { get; set; }
        public string responsecode { get; set; }
    }
    /// <summary>
    /// Reference List
    /// </summary>
    public class Referencelist
    {
        public string checknumber { get; set; }
        public string createddate { get; set; }
        public string createdtime { get; set; }
        public string dewareferencenumber { get; set; }
        public string nocheckpay { get; set; }
        public string okpay { get; set; }
        public string paidaccountsreferencenumber { get; set; }
        public string paidamountreferencenumber { get; set; }
        public string postingdate { get; set; }
        public string status { get; set; }
        public string time { get; set; }
        public string totalaccounts { get; set; }
        public string totalamount { get; set; }
    }
}
