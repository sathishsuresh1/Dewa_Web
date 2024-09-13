using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.Bills
{
	public class RequestCollectiveAccount
	{
		public string BusinessPartnerNumber { get; set; }
		
		[DataType(DataType.EmailAddress)]
		[DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
		[DataAnnotations.EmailAddress(ValidationMessageKey = "email validation message")]
		public string EmailAddress { get; set; }

		[DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
		public string ContactName { get; set; }

		[DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "uae mobile number validation message")]
        [DataAnnotations.RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string Mobile { get; set; }

		public string SelectedCategoryKey { get; set; }

		[DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
		public string SelectedCategoryValue { get; set; }
		
		public Dictionary<string, string> Categories { get; set; }

		public HttpPostedFileBase OfficialLetterUploader { get; set; }

		public HttpPostedFileBase TradeLicenceUploader { get; set; }

		public string NotificationNumber { get; set; }

	}
}