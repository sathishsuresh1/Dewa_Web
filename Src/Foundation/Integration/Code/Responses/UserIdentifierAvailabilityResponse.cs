using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
	[XmlRoot(ElementName = "UserIDAvailability")]
	public class UserIdentifierAvailabilityResponse
	{
		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }

		[XmlElement(ElementName = "UserID")]
		public string UserIdentifier { get; set; }

		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }

		public bool IsAvailableForUse
		{
			get { return ResponseCode == 0; }
		}
	}
}
