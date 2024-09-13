
using System;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{

	[Serializable]
	[XmlRoot(ElementName = "UploadphotoResponse")]
	public class UploadPhotoResponse
	{
		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }
		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "DateTimeStamp")]
		public string DateTimeStamp { get; set; }
	}
}
