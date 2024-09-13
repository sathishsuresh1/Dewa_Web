using DEWAXP.Foundation.DataAnnotations;
using System;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class ForgotPasswordModel
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Username { get; set; }


        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string Email { get; set; }

        public string OtpRequestId { get; set; }

        public string ReferenceId { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; }

        public bool isPasswordSetSuccessful { get; set; }

        public string CustomerType { get; set; }
       
    }
}