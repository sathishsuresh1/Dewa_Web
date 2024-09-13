using System;
using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
	public class VerifyEmailandMobileModel
    {
        public string EmailAddess { get; set; }
        public string Mobile { get; set; }
        public string SelectedOption { get; set; }
        public string Username { get; set; }
        
    }
}