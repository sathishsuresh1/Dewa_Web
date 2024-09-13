using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.USC.Models.SmartCommunications
{
    public class ConsumptionVerification
    {
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string AccountNumber { get; set; }
        public string SelectedOptions { get; set; }
        public string OTPKey { get; set; }
    }
}