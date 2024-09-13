using System;
using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
	[Serializable]
	public class ForgotPasswordV1Model
    {
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
		public string Username { get; set; }
	}
}