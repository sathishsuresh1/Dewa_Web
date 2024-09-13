using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    public class SetNewPasswordModel
    {
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [MaxLength(50)]
        public string CurrentPassword { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Scholarship Password Format Validation Failed")]
        [RegularExpression(@"^(?=(.*\d){1})(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\d]).{8,40}$",
            AllowEmptyStrings = false, ValidationMessageKey = Constants.DictionaryKeys.PASSWORD_FORMAT_VALIDATION)]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Compare("Password", ValidationMessageKey = "Password and Repeat Password Must Match")]
        public string ConfirmPassword { get; set; }

        public bool PasswordResetSuccessful = false;
        //public string Message { get; set; }
    }
}