using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Content.Models.RERA;

namespace DEWAXP.Feature.SupplyManagement.Models.RERA
{
    [Serializable]
    public class ReraUserRegistrationModel
    {
        public ReraSetUsernamePasswordModel UsernamePasswordModel { get; set; }
        public moveInPostOutput PayLoad { get; set; }
    }

    [Serializable]
	public class ReraSetUsernamePasswordModel : ReraCustomerDetails
    {
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string VerificationCode { get; set; }

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

	}
}