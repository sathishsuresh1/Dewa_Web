using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.Enums;

namespace DEWAXP.Foundation.Integration.Requests
{
    public class LodgeBillingComplaint
    {
        public string PremiseNumber { get; set; }

        public string ContractAccountNumber { get; set; }

        public string BusinessPartnerNumber { get; set; }

        public string MobileNumber { get; set; }

        public string Remarks { get; set; }

        public byte[] Attachment { get; set; }

        public string AttachmentExtension { get; set; }

        public MunicipalService AffectedService { get; set; }

        public ComplaintPriority Priority { get; set; }
    }
}
