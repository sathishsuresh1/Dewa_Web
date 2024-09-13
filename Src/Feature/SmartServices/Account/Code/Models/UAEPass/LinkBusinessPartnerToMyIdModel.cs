namespace DEWAXP.Feature.Account.Models.UAEPass
{
	public class LinkBusinessPartnerToMyIdModel
	{
		[Foundation.DataAnnotations.Required(ValidationMessageKey = "generic validation message")]
		public string MyId { get; set; }

		[Foundation.DataAnnotations.Required(ValidationMessageKey = "generic validation message")]
		public string EmiratesIdentifier { get; set; }

		[Foundation.DataAnnotations.Required(ValidationMessageKey = "Business partner number validation error")]
		public string BusinessPartnerNumber { get; set; }

		[Foundation.DataAnnotations.Required(ValidationMessageKey = "email validation message")]
		[Foundation.DataAnnotations.EmailAddress(ValidationMessageKey = "email validation message")]
		public string EmailAddress { get; set; }

		[Foundation.DataAnnotations.Required(ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [Foundation.DataAnnotations.RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }

		[Foundation.DataAnnotations.Required(ValidationMessageKey = "generic validation message")]
		public string PoBox { get; set; }

	}
}