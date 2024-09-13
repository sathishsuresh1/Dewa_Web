using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Feature.SupplyManagement.ModelBinders;

namespace DEWAXP.Feature.SupplyManagement.Models.RequestTempConnection
{

    [Bind(Include = "AccountNumber,MobileNumber,Location,EventType,EventStartDate,EventEndDate,Description,TermsConditions")]
    [ModelBinder(typeof(RequestTempConnectionModelBinder))]
    public class NewRequest
	{
		public string AccountNumber { get; set; }
		
		[Required]
		public string MobileNumber { get; set; }

		[Required]
		public string Location { get; set; }

		[Required]
		public EventType EventType { get; set; }

		[Required]
		public DateTime? EventStartDate { get; set; }

		[Required]
		public DateTime? EventEndDate { get; set; }

        
		public string Description{ get; set; }
		public bool TermsConditions { get; set; }
	}
}