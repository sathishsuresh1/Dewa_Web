using System;
using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
	[Serializable]
	public class LoginModel
	{
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
		public string Username { get; set; }

		[DataType(DataType.Password)]
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
		//[Foundation.DataAnnotations.MinLength(8, ValidationMessageKey = "login password validation message")]
		public string Password { get; set; }
	}
}