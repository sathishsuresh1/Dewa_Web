using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.Payment
{
	[Serializable]
	public class PaymentMetaDataModel
	{
		public PaymentMetaDataModel()
		{
			BusinessPartnerNumbers = new string[0];
			ContractAccountNumbers = new string[0];
		}

		public string[] BusinessPartnerNumbers { get; set; }

		public string[] ContractAccountNumbers { get; set; }
	}
}