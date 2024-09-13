using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Integration.CustomerSmartSalesSvc;
using System;

namespace DEWAXP.Foundation.Content.Models.ScrapSale
{
    [Serializable]
    public class ScrapeTenderPaymentModel : openTender
    {
        public bool PaymentStatus { get; set; }

        public string ReferenceNo { get; set; }

        public bool IsOnline { get; set; }

        public bool IsSuccess { get; set; }

        public bool IsJustPaid { get; set; }
        public string DegTransactionId { get; set; }
        public string DewaTransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Message { get; set; }
        public string Epnumber { get; set; }
        public string Eptype { get; set; }
        public string ReceiptIdentifiers { get; set; }
        public bool IsError { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
        public decimal PaidSuqiaAmount { get; set; }
        public decimal PaidAmount { get; set; }
    }
}