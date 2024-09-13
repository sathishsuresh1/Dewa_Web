using DEWAXP.Foundation.Content.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Bills.Models.Tayseer
{
    public class TayseerModel
    {
        public string LineNo { get; set; }
        public string Error { get; set; }
        public string Data { get; set; }
    }
    public class TayseerDetail
    {
        /// <summary>  
        /// To hold list of orders  
        /// </summary>  
        public List<TayseerModel> TayseerDetails { get; set; }
    }
    public class TayseerPayModel
    {
        public string EasyPayNumber { get; set; }
        public string Name { get; set; }
        public string ServiceType { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public decimal TotalAmount { get; set; }
        public string PartialPayFlage { get; set; }
        public string Transactiontype { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }
}