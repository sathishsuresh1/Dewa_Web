using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
	[XmlRoot(ElementName = "GetGovtObservationDetails")]
	public class GetGovernmentObservationDetailsResponse
	{
		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }

		[XmlElement(ElementName = "ObservationNo")]
		public string ObservationNo { get; set; }

		[XmlElement(ElementName = "Subrc")]
		public string Subrc { get; set; }

		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }
	}
}
