

using System;
using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
    [Serializable]
    public class ForgotPasswordModel
    {
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
        [Foundation.DataAnnotations.EmailAddress(ValidationMessageKey = "email validation message")]
        public string Email { get; set; }
    }
}