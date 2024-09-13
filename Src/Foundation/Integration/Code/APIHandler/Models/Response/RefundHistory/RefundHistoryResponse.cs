using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.RefundHistory
{
    public class RefundHistoryResponse
    {
        public string businesspartnernumber { get; set; }
        public string description { get; set; }
        public List<RefundHistoryEmailResponse> emaillist { get; set; }
        public List<RefundHistoryMobileResponse> mobilelist { get; set; }
        public string responseCode { get; set; }
        public string flag { get; set; }
        public string maxattempts { get; set; }
    }

    public class RefundHistoryEmailResponse
    {
        public string maskedemail { get; set; }
        public string unmaskedemail { get; set; }
    }

    public class RefundHistoryMobileResponse
    {
        public string maskedmobile { get; set; }
        public string unmaskedmobile { get; set; }
    }
}

