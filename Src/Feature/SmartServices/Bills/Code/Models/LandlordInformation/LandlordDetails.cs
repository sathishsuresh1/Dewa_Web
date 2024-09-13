using DEWAXP.Foundation.DataAnnotations;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Models.LandlordInformation
{
	public class LandlordDetails
	{
		[Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
		public string SelectedBusinessPartnerNumber { get; set; }

		[Required(AllowEmptyStrings = false, ValidationMessageKey = "premise number validation message")]
		public string PremiseNumber { get; set; }

		[Required(AllowEmptyStrings = false, ValidationMessageKey = "uae mobile number validation message")]
		public string Mobile { get; set; }

		[Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
		[MinLength(0, ValidationMessageKey = "min length validation message")]
		[MaxLength(500, ValidationMessageKey = "max length validation message")]
		public string Remarks { get; set; }

		public List<SelectListItem> BusinessPartnerNumberList { get; set; }

		public HttpPostedFileBase OfficialLetterUploader { get; set; }

		public HttpPostedFileBase TitleDeedUploader { get; set; }

		public string ReferenceNumber { get; set; }
	}
}