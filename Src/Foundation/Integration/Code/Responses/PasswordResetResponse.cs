using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "ResetCustomerPortalPassword")]
    public class PasswordResetResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "Email")]
        public string Email { get; set; }

		[XmlRoot(ElementName = "PasswordValidate")]
		public class PasswordValidate
		{
			[XmlElement(ElementName = "ResponseCode")]
			public string ResponseCode { get; set; }
			[XmlElement(ElementName = "Description")]
			public string Description { get; set; }
		}
    }
}


