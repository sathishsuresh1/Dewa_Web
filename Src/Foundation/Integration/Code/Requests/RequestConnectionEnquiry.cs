using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests
{
    [Serializable]
    public class RequestConnectionEnquiry
    {
        public string UserId { get; set; }
        public string PremiseNo { get; set; }
        public string ContractAccountNo { get; set; }
        public string BusinessPartnerNo { get; set; }
        public string MobileNo { get; set; }
        public string Remarks { get; set; }
        public byte[] Attachment { get; set; }
        public string AttachmentType { get; set; }
        public string QueryType { get; set; }
        public string SessionNo { get; set; }

    }
}
