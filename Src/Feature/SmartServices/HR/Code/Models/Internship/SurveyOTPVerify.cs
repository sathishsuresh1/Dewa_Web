using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.Internship
{
    [Serializable]
    public class SurveyOTPVerify
    {
        public string MobileNumber { get; set; }
        public string MaskedMobileNumber { get; set; }
        public string MaskedEmailAddress { get; set; }
        public string EmailAddress { get; set; }
        public string SelectedOptions { get; set; }
        public bool OTPSend { get; set; } = false;
        public bool OTPVerified { get; set; } = false;
        public string Message { get; set; }
        public string TimerTime { get; set; }
    }
}