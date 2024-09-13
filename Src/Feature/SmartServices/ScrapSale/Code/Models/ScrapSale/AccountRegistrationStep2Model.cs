using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class AccountRegistrationStep2Model
    {
        public string MaskedEmail { get; set; }
        public string SendBY { get; set; }
        public string MaskedMobile { get; set; }
        public string ConsumerNo { get; set; }
       //public bool OtpTrigger { get; set; }
        public string OtpNumber { get; set; }

        public string ActionType { get; set; }

    }
}