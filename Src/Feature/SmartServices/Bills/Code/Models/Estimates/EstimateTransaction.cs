using DEWAXP.Foundation.Integration.DewaSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SitecoreX = Sitecore.Context;
using _commonUtility = DEWAXP.Foundation.Content.Utils.CommonUtility;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    public class EstimateTransaction
    {


        public string AccountNumber { get; set; }
        public string Amount { get; set; }
        public decimal FormattedAmount { get; set; }
        public string ApplicationNumber { get; set; }
        public string Channel { get; set; }
        public string ChequeReturn { get; set; }
        public string CurrencyKey { get; set; }
        public string DocumentNumber { get; set; }
        public string Name { get; set; }
        public string Noreceipt { get; set; }
        public string PaymentDescription { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentType { get; set; }
        public string TaxInvoiceApplicability { get; set; }
        public DateTime DateTime { get; set; }
        public string Timestamp { get; set; }
        public string FormattedDate => DateTime.ToString(_commonUtility.DF_dd_MMM_yyyy, SitecoreX.Culture);
        public string FormattedDateTime => DateTime.ToString(_commonUtility.DF_dd_MMM_yyyy_HHmmss, SitecoreX.Culture);
        public string TransactionId { get; set; }
        public static EstimateTransaction From(paymentDetails pd)
        {
            EstimateTransaction et = new EstimateTransaction();
            et.TransactionId = pd.transactionid;
            et.AccountNumber = pd.accountnumber;
            et.Amount = pd.amount;
            decimal amount = decimal.Zero;
            decimal.TryParse(pd.amount.Trim(), out amount);
            et.FormattedAmount = amount;
            et.ApplicationNumber = pd.applicationnumber;
            et.Channel = pd.channel;
            et.ChequeReturn = pd.chequereturn;
            et.CurrencyKey = pd.currencykey;
            et.DocumentNumber = pd.documentnumber;
            et.Name = pd.name;
            et.Noreceipt = pd.noreceipt;
            et.PaymentDescription = pd.paymentdescription;
            et.PaymentMode = pd.paymentmode;
            et.PaymentType = pd.paymenttype;
            et.TaxInvoiceApplicability = pd.taxinvoiceapplicability;
            et.Timestamp = pd.timestamp;
            et.DateTime = DateTime.ParseExact(et.Timestamp, _commonUtility.DF_yyyyMMddHHmmss, System.Globalization.CultureInfo.InvariantCulture);
            return et;
        }
    }
}