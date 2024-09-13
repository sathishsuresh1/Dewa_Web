using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Events.Models.DisabilityConference
{
	[Serializable]
	public class DisabilityConferenceModel
	{
		public string Title { get; set; }
		public string Industry { get; set; }

		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string Nationality { get; set; }
		public string TelephoneNumber { get; set; }
		public string FaxNumber { get; set; }
		public string Company { get; set; }
		public string JobTitle { get; set; }
		public string Department { get; set; }
		public string CompanyWebsite { get; set; }
		public string Country { get; set; }
		public string POBOX { get; set; }
		public string Address { get; set; }

        [RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }
		
		public string EmailAddress { get; set; }
		
		public List<SelectListItem> TitleList { get; set; }
		public List<SelectListItem> IndustryList { get; set; }

		public string IsInterested { get; set; }
		
	}
}