using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate
{
    public class PaymentReceiptDetailsRequest
    {
        public string appidentifier { get; set; }
        public string transactionid { get; set; }
        public string appversion { get; set; }
        public string paymenttypetext { get; set; }
        public string lang { get; set; }
        public string datetimestamp { get; set; }
        public string mobileosversion { get; set; }
        public string channel { get; set; }
        public string sessionid { get; set; }
        public string userid { get; set; }
        public string vendorid { get; set; }
    }
}
