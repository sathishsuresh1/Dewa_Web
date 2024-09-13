using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.Tayseer
{
    /// <summary>
    /// Tayseer Detail Response
    /// </summary>
    public class TayseerDetailsResponse
    {
        public List<Accountlist> accountlist { get; set; }
        public string bankreferencenumber { get; set; }
        public string checknumber { get; set; }
        public string createddate { get; set; }
        public string createdtime { get; set; }
        public string description { get; set; }
        public string nocheckpay { get; set; }
        public string okaypay { get; set; }
        public string paidaccountsreferencenumber { get; set; }
        public string paidamountreferencenumber { get; set; }
        public string postingdate { get; set; }
        public string postingtime { get; set; }
        public string responsecode { get; set; }
        public string status { get; set; }
        public string totalaccounts { get; set; }
        public string totalamount { get; set; }
    }
    /// <summary>
    /// Account List
    /// </summary>
    public class Accountlist
    {
        public string amount { get; set; }
        public string amountpaid { get; set; }
        public string collectiveaccount { get; set; }
        public string contractaccount { get; set; }
    }
}
