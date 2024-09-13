using System;
using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
	public class ForgotPasswordViewModel
    {
        public string Businesspartnernumber { get; set; }
        public string Username { get; set; }
        public string EmailAddess { get; set; }
        public string MaskedEmailAddess { get; set; }
        public string Mobile { get; set; }
        public string MaskedMobile { get; set; }
        public string SelectedOption { get; set; }
        public string OTP { get; set; }
        public string ValidityMinutes { get; set; }
        public string ValiditySeconds { get; set; }
    }
}