using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.CareerPortal
{
    public class TellAFriend
    {
       
        public string JobId { get; set; }
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string RecipentName { get; set; }
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string RecipentEmailAddress { get; set; }
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string SenderName { get; set; }
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string SenderMessage { get; set; }
    }
}