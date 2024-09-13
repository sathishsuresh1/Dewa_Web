using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests
{
	[Serializable]
    public class RequestClearanceCertificate
    {
        public string FirstName { get; set; }
		
        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string MobileNumber { get; set; }

        public string IdentityNumber { get; set; }

        public string TradeLicenseNumber { get; set; }

		public string TradeLicenseAuthority { get; set; }

		public string PassportNumber { get; set; }
		
        public string PoBox { get; set; }

        public string City { get; set; }
		
        public string ContractAccountNumber { get; set; }
		
        public string Purpose { get; set; }

		public string Emirates { get; set; }

		public string Remarks { get; set; }

        public string Branch { get; set; }

        public string CourtNumber { get; set; }

        public byte[] Attachment1 { get; set; }

        public string Attachment1Extension { get; set; }

        public byte[] Attachment2 { get; set; }

        public string Attachment2Extension { get; set; }

        public byte[] Attachment3 { get; set; }

        public string Attachment3Extension { get; set; }

        public byte[] Attachment4 { get; set; }

        public string Attachment4Extension { get; set; }

		public decimal TotalPayable { get; set; }

        public string @cclang { get; set; }
    }
}
