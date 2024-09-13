using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Feature.Account.Models.CustomerRegistration
{
	public class VerifyModel : ConfirmVerificationMethodModel
	{
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
		public string VerificationCode { get; set; }
        public string maxAttempt { get; set; }
	}
}