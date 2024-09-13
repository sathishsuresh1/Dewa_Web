using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Feature.EV.Models.EVCharger
{
    public class EVBPAccountSetup
    {
        public string BusinessPartnerNumber { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        //[Foundation.DataAnnotations.MinLength(8, ValidationMessageKey = "login password validation message")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Foundation.DataAnnotations.Compare("Password", ValidationMessageKey = "Password mismatch error")]
        public string ConfirmPassword { get; set; }

        public bool isAgreeStatus { get; set; }
    }
}