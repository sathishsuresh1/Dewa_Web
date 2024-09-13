using DEWAXP.Foundation.DataAnnotations;
using System;
using Validation = Sitecore.Shell.Feeds.Sections.Validation;
	
namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class ForgotUsernameModel
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        [MinLength(8, ValidationMessageKey = "register customer number validation message")]
        public string CustomerNumber { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string Email { get; set; }   
        
        public string CustomerType { get; set; }


    }
}