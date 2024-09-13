using DEWAXP.Foundation.DataAnnotations;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Models.LandlordInformation
{
	public class LandlordSupplyActivation
    {
		[Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
		public string SelectedBusinessPartnerNumber { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "account number validation message")]
        public string AccountNumber { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "premise number validation message")]
		public string PremiseNumber { get; set; }

		[Required(AllowEmptyStrings = false, ValidationMessageKey = "uae mobile number validation message")]
		public string Mobile { get; set; }

		public List<SelectListItem> BusinessPartnerNumberList { get; set; }

        public string Nationality { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid email address")]
        public string Email { get; set; }

        public string SuccessMessage { get; set; }
    }
}