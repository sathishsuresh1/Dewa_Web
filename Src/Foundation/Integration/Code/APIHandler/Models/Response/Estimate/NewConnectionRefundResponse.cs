using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate
{
    public class NewConnectionRefundResponse
    {
        public string description { get; set; }
        public string emailaddress { get; set; }
        public string fullname { get; set; }
        public List<NewConnectionRefundIbandetail> ibandetails { get; set; }
        public List<NewConnectionRefundConnectiondetail> connectiondetails { get; set; }
        public string mobile { get; set; }
        public string notificationnumber { get; set; }
        public string okcheque { get; set; }
        public string okiban { get; set; }
        public string responsecode { get; set; }
        public string attachflag { get; set; }

        public bool IsServiceFailure { get; set; }
    }

    public class NewConnectionRefundIbandetail
    {
        public string iban { get; set; }
        public string maskiban { get; set; }
        public string sequenceno { get; set; }
    }

    public class NewConnectionRefundConnectiondetail
    {
        public string applicationno { get; set; }
        public string posid { get; set; }
    }
}
