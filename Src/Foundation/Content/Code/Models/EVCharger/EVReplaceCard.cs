using DEWAXP.Foundation.Content.Models.Payment;

namespace DEWAXP.Foundation.Content.Models.EVCharger
{
    public class EVReplaceCard
    {
        public string AccountNumber { get; set; }
        public string CardNumber { get; set; }
        public string RequestNumber { get; set; }
        public string Reason { get; set; }
        public string TotalAmount { get; set; }
        public string TaxAmount { get; set; }
        public string TaxRate { get; set; }
        public string CardFee { get; set; }
        public string CourierFee { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }
}