using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses
{
	[XmlRoot(ElementName = "GetTemporaryConnectionDetails")]
    public class TemporaryConnectionDetails
    {
		private string _contractAccountNumber;

		[XmlElement(ElementName = "DateTimeStamp")]
        public string DateTimeStamp { get; set; }

        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "Mobile")]
        public string MobileNumber { get; set; }

		[XmlElement(ElementName = "ContractAccount")]
		public string ContractAccountNumber
		{
            get { return DewaResponseFormatter.Trimmer(_contractAccountNumber); }
			set { _contractAccountNumber = value ?? string.Empty; }
		}

		[XmlElement(ElementName = "NotificationText")]
        public string NotificationText { get; set; }

        [XmlElement(ElementName = "EventType")]
        public string EventType { get; set; }

        [XmlElement(ElementName = "NotificationNumber")]
        public string NotificationNumber { get; set; }

        [XmlElement(ElementName = "FromDate")]
        public string FromDate { get; set; }

        [XmlElement(ElementName = "ToDate")]
        public string ToDate { get; set; }

        [XmlElement(ElementName = "City")]
        public string City { get; set; }

        [XmlElement(ElementName = "NotificationStatus")]
        public string NotificationStatus { get; set; }

        [XmlElement(ElementName = "TaskStatus")]
        public string TaskStatus { get; set; }

        [XmlElement(ElementName = "Amount")]
        public decimal Amount { get; set; }

        [XmlElement(ElementName = "TaskKey")]
        public string TaskKey { get; set; }

        [XmlElement(ElementName = "PayButton")]
        public string PayButton { get; set; }

        [XmlElement(ElementName = "PayDate")]
        public string PayDate { get; set; }

        [XmlElement(ElementName = "NetAmount")]
        public decimal NetAmount { get; set; }

        [XmlElement(ElementName = "TaxAmount")]
        public decimal TaxAmount { get; set; }
    }

}
