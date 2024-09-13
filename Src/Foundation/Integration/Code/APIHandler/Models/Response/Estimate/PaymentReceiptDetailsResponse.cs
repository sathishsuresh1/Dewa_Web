using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate
{
    public class PaymentReceiptDetailsResponse
    {
        public string applicationnumber { get; set; }
        public string channel { get; set; }
        public string datetimestamp { get; set; }
        public string description { get; set; }
        public string dewareferencenumber { get; set; }
        public string dewatransactionid { get; set; }
        public string paymentsource { get; set; }
        public string paymenttypetext { get; set; }
        public List<Receiptaccountlist> receiptaccountlist { get; set; }
        public string receiptdescription { get; set; }
        public string responsecode { get; set; }
        public string sdgtransactionid { get; set; }
        public string totalaccount { get; set; }
        public string totalamount { get; set; }
    }
    public class Receiptaccountlist
    {
        public string amount { get; set; }
        public string contractaccountnumber { get; set; }
        public string currencykey { get; set; }
        public string name { get; set; }
    }
}
