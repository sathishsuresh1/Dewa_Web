using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.Payment.SecurePay
{
    public class CipherPaymentModel
    {
        public CipherPaymentModel() {
            PaymentData = new Integration.APIHandler.Models.Request.SecuredPayment.CipherPaymentDetailInputs();
            PayPostModel = new CipherPaymentPostModel();
        }
        public Integration.APIHandler.Models.Request.SecuredPayment.CipherPaymentDetailInputs PaymentData { get; set; }

        #region [Custom variable]
        public bool IsThirdPartytransaction { get; set; }
        public ServiceType? ServiceType { get; set; }
        public PaymentContext ContextType
        {
            get
            {
                return WebHandler.CipherPaymentHandler.GetPaymentContext(ServiceType.Value, IsThirdPartytransaction);
            }
        }

        public IDictionary<string, string> ErrorMessages { get; set; }
        public CipherPaymentPostModel PayPostModel { get; set; }
        public bool IsValidAccountsAndAmountDetails()
        {
            if (!string.IsNullOrWhiteSpace(PaymentData.contractaccounts) && !string.IsNullOrWhiteSpace(PaymentData.amounts))
            {
                var _accounts = PaymentData.contractaccounts.Split(',');
                var _amounts = PaymentData.amounts.Split(',');

                return _accounts.Length == _amounts.Length;
            }
            return false;
        }
        public PaymentMethod PaymentMethod { get; set; }
        public string BankKey { get; set; }
        public string SuqiaValue { get; set; }
        public string SuqiaAmt { get; set; }
        #endregion
    }
    public class CipherPaymentPostModel
    {
        public string PaymentUrl { get; set; }

        public string dewatoken { get; set; }
        public string u { get; set; }
    }
}