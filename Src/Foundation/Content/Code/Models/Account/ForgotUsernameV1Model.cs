using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Validation = Sitecore.Shell.Feeds.Sections.Validation;
using System.ComponentModel.DataAnnotations;
using DEWAXP.Foundation.DataAnnotations;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
    [Serializable]
    public class ForgotUsernameV1Model
    {
        [DataType(DataType.EmailAddress)]
        //[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
        [Foundation.DataAnnotations.EmailAddress(ValidationMessageKey = "email validation message")]
        public string Email { get; set; }


        [EmiratesID(ValidationMessageKey = "emiratesid validation message")]
        public string EmiratesID { get; set; }

        //[Foundation.DataAnnotations.Required(AllowEmptyStrings = true, ValidationMessageKey = "generic validation message")]
        public string PassportNo { get; set; }

        public string SelectedOption { get; set; }
    }
    
    
}