using System;
using System.ComponentModel.DataAnnotations;
using Validation = Sitecore.Shell.Feeds.Sections.Validation;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
	[Serializable]
	public class ForgotUsernameModel
	{
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
		[Foundation.DataAnnotations.MinLength(8, ValidationMessageKey = "register business partner validation message")]
		public string BusinessPartnerNumber { get; set; }

		[DataType(DataType.EmailAddress)]
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
		[Foundation.DataAnnotations.EmailAddress(ValidationMessageKey = "email validation message")]
		public string Email { get; set; }
	}
}