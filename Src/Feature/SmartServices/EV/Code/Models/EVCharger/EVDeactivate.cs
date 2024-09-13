using DEWAXP.Foundation.Content.Models.Payment;
using System;

namespace DEWAXP.Feature.EV.Models.EVCharger
{
    [Serializable]
    public class EVDeactivate
    {
        public string AccountNumber { get; set; }
        public string BPNumber { get; set; }
        public string DeactivateDate { get; set; }
        public string MobileNumber { get; set; }

        public string cardNumber { get; set; }

        public DateTime? Deactivatedt { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
        public string bankkey { get; set; }
    }
}