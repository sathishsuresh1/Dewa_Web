using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    public class ForgotPassword
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string UserName { get; set; }
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string EmailAddress { get; set; }
    }
}