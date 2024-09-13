using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.DewaAcademy
{
    public class Login
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string EmailAddress { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        //[MinLength(8, ValidationMessageKey = "login password validation message")]
        public string Password { get; set; }
    }
}