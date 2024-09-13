using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class SetNewPasswordModel
    {
        public string SessionToken { get; set; }

        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        [RegularExpression("^(?=.*\\d)(?=.*[\\D])[0-9\\D]{8,}$", ValidationMessageKey = "login password validation message alphanumeric")]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Compare("Password", ValidationMessageKey = "Password mismatch error")]
        public string ConfirmPassword { get; set; }

        public string Key { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string MaskedEmail { get; set; }

        public string MaskedMobile { get; set; }

        public string OtpRequestId { get; set; }
        public string SMSDuration { get; set; }
        public string EmailDuration { get; set; }
        public string MaxAttemptflag { get; set; }
        public bool isSucess { get; set; }
        public string Otp { get; set; }


    }
}