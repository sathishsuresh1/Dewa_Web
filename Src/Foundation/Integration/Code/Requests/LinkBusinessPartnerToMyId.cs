using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests
{
	public class LinkBusinessPartnerToMyId
	{
		public string BusinessPartnerNumber { get; set; }

		public string MobileNumber { get; set; }

		public string EmailAddress { get; set; }

		public string PoBox { get; set; }

		public string MyIdUsername { get; set; }
		
		public string EmiratesIdentifier { get; set; }
	}
}
