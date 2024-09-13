using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Feature.Account.Models.CustomerRegistration
{
	public class SetUsernamePasswordModel
	{
		public string BusinessPartnerNumber { get; set; }

		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "register username validation message")]
		[Foundation.DataAnnotations.MinLength(6, ValidationMessageKey = "register username validation message")]
		[Foundation.DataAnnotations.MaxLength(75, ValidationMessageKey = "register username validation message")]
		public string Username { get; set; }

		[DataType(DataType.Password)]
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
		[Foundation.DataAnnotations.MinLength(8, ValidationMessageKey = "login password validation message")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Foundation.DataAnnotations.Compare("Password", ValidationMessageKey = "Password mismatch error")]
		public string ConfirmPassword { get; set; }

		//[Foundation.DataAnnotations.EmailAddress(ValidationMessageKey = "email validation message", AllowEmptyStrings = true)]
		public string Email { get; set; }

        [Foundation.DataAnnotations.RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string Mobile { get; set; }

		public bool CanUpdateEmailAddress { get; set; }

		public bool CanUpdateMobileNumber { get; set; }
	}
}