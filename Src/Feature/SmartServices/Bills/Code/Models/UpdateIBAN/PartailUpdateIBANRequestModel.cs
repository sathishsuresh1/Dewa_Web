using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Bills.UpdateIBAN
{
    public class PartailUpdateIBANRequestModel
    {
        public string SelectedAccount { get; set; }
        public string SelectedRefernceNo { get; set; }
        public string SelectedBusinessPartnerNumber { get; set; }
        public string IBANNumber { get; set; }
        public string IBANConfirmNumber { get; set; }
        public string Reference { get; set; }
        public string EmailAddess { get; set; }
        public string MaskedEmailAddess { get; set; }
        public string Mobile { get; set; }
        public string MaskedMobile { get; set; }
        public string OTP { get; set; }
        public bool IsSuccess { get; set; }
    }
}