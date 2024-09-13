using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.CareerPortal
{
    public class POD
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string FullName { get; set; }
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string EmailAddress { get; set; }
        public string CountryCode { get; set; }

        public string MobileNumber { get; set; }
        public string RepacthaValue { get;  set; }
        public string PodID { get; set; }
    }
}