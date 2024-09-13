using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
	[XmlRoot(ElementName = "GetEmailListForMobile")]
	public class MarketingEmailListResponse
	{
		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }

		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }

		[XmlElement(ElementName = "ListOfEmails")]
		public MarketingEmails ListOfEmails { get; set; }
	}

	[XmlRoot(ElementName = "ListOfEmails")]
	public class MarketingEmails
	{
		[XmlElement(ElementName = "UserEmail")]
		public List<MarketingEmailDetails> Account { get; set; }
	}

	[XmlRoot(ElementName = "UserEmail")]
	public class MarketingEmailDetails
	{
		[XmlElement(ElementName = "BpNumer")]
		public string BpNumber { get; set; }

		[XmlElement(ElementName = "Email")]
		public string Email { get; set; }
	}
}
