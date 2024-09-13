using System;
using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
	[Serializable]
	public class ChangePasswordModel
	{
		public string UserId { get; set; }

		[DataType(DataType.Password)]
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "Current password validation message")]
		public string OldPassword { get; set; }

		[DataType(DataType.Password)]
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        [Foundation.DataAnnotations.RegularExpression("^(?=.*\\d)(?=.*[\\D])[0-9\\D]{8,}$", ValidationMessageKey = "login password validation message alphanumeric")]
        [Foundation.DataAnnotations.NonEqual("UserId", ValidationMessageKey = "Password cannot match username")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Foundation.DataAnnotations.Compare("NewPassword", ValidationMessageKey = "New password and confirmation mismatch error")]
		public string ConfirmPassword { get; set; }

        public string TabId { get; set; }
	}
}