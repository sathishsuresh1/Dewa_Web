using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.DataAnnotations;
using System;

namespace DEWAXP.Feature.Bills.Models.EasyPay
{
    [Serializable]
    public class EasyPayModel
    {
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter EasyPay number")]
        [MaxLength(10, ValidationMessageKey = "Please enter EasyPay number")]
        public string EasyPayNumber { get; set; }

        public string Name { get; set; }
        public string ServiceType { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public decimal TotalAmount { get; set; }
        public string PartialPayFlage { get; set; }
        public string Transactiontype { get; set; }
        public EasyPayTransactionType epTrans { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }

    public enum EasyPayTransactionType
    {
        BILL,
        COLBILL,
        REFBILL,
        ESTMNM
    }
}