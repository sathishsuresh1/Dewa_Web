using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Events.Models.BestConsumerAward
{
	[Serializable]
	public class BestConsumerAwardModel
	{
		[MaxLength(10, ValidationMessageKey = "Valid Contract Account")]
		[MinLength(10, ValidationMessageKey = "Valid Contract Account")]
		public string ContractAccountNumber { get; set; }
		
		public string AccountOwnerName { get; set; }

        [RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }
		
		public string EmailAddress { get; set; }
		public string ResidenceAddress { get; set; }
		public string TypeResidence { get; set; } 

		
		public string TypeAC { get; set; } 
		
		public string NumberResidents {get;set;}
		public string Numberbedrooms { get; set; }
		public string Numberbathrooms { get; set; }
		public string NumberCars { get; set; }
		public string Measures {get;set;}
		public string SourceType { get; set; }
		public string InstituteName { get; set; }
		public List<SelectListItem> ResidenceList { get; set; }
		public List<SelectListItem> ACList { get; set; }
		
	}
}