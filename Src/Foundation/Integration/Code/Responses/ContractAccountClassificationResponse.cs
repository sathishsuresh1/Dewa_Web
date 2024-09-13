using System;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.DubaiModelSvc;
using DEWAXP.Foundation.Integration.Enums;

namespace DEWAXP.Foundation.Integration.Responses
{
	[Serializable]
	[XmlRoot(ElementName = "getAccountTypeStatusResponse")]
	public class ContractAccountClassificationResponse
	{
		[XmlElement(ElementName = "responseCode")]
		public int ResponseCode { get; set; }

		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		
		[XmlElement(ElementName = "connectionType")]
		public string ConnectionType { get; set; }

		[XmlElement(ElementName = "billingClass")]
		public string BillingClassCode { get; set; }

        public string PremiseType { get; set; }

		public BillingClassification BillingClass
		{
			get
			{
				if (!string.IsNullOrEmpty(BillingClassCode))
				{
					return (BillingClassification)int.Parse(BillingClassCode);
				}
				return BillingClassification.Unknown;
			}
		}

		public static ContractAccountClassificationResponse From(accountTypeStatusRes payload)
		{
			return new ContractAccountClassificationResponse
			{
				ResponseCode = int.Parse(payload.responseCode),
				Description = payload.description,
				BillingClassCode = payload.billingClass,
				ConnectionType = payload.connectionType,
                PremiseType = payload.premiseType
			};
		}
	}
}
