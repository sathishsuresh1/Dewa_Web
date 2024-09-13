using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
	public class SetNewPasswordV1Model
    {
		public string Username { get; set; }

		[DataType(DataType.Password)]
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        [Foundation.DataAnnotations.RegularExpression("^(?=.*\\d)(?=.*[\\D])[0-9\\D]{8,}$", ValidationMessageKey = "login password validation message alphanumeric")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Foundation.DataAnnotations.Compare("Password", ValidationMessageKey = "Password mismatch error")]
		public string ConfirmPassword { get; set; }
	}
}