using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.SecuredPayment
{
    public class CipherPaymentDetailResponse
    {
        public string description { get; set; }
        public string paymenttoken { get; set; }
        public string responsecode { get; set; }
    }
}
