using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate;
using DEWAXP.Foundation.Integration.DewaSvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    public class EstimateReceiptDetailModel
    {
        public DateTime Date { get; set; }

        public string FormattedDate => Date.ToString("dd MMM yyyy", SitecoreX.Culture);

        public string FormattedDateTime => Date.ToString("dd MMM yyyy | HH:mm:ss", SitecoreX.Culture);

        public decimal Amount { get; set; }

        public string Channel { get; set; }

        public string Description { get; set; }

        public string PaymentTypetext { get; set; }

        public string PaymentType { get; set; }

        public string DocumentNumber { get; set; }

        public string DewaTransactionReference { get; set; }

        public string Dewatransactionid { get; set; }

        public string ReceiptDescription { get; set; }

        public string SDGTransactionId { get; set; }

        public string TotalAccount { get; set; }

        public int Totalpage { get; set; }

        public bool Viewmore { get; set; }

        public bool Isdocumentnumber { get; set; }

        public bool IstransactionId { get; set; }

        public List<ReceiptAccountModel> ReceiptAccounts { get; set; }
        public string ApplicationNumber { get; private set; }
        public string PaymentMode { get; private set; }

        public static EstimateReceiptDetailModel From(PaymentReceiptDetailsResponse paymentreceipt, int page = 1)
        {
            EstimateReceiptDetailModel paymentmodel = new EstimateReceiptDetailModel();
            decimal amount = decimal.Zero;
            decimal.TryParse(paymentreceipt.totalamount.Trim(), out amount);
            paymentmodel.IstransactionId = true;
            paymentmodel.Amount = amount;
            paymentmodel.Channel = paymentreceipt.channel;
            //paymentmodel.Description = paymentreceipt.description;
            paymentmodel.DewaTransactionReference = paymentreceipt.dewareferencenumber;
            paymentmodel.Dewatransactionid = paymentreceipt.dewatransactionid;
            paymentmodel.Description = paymentreceipt.receiptdescription;
            paymentmodel.TotalAccount = paymentreceipt.totalaccount;
            paymentmodel.SDGTransactionId = paymentreceipt.sdgtransactionid;
            paymentmodel.PaymentType = paymentreceipt.paymentsource;
            paymentmodel.PaymentTypetext = paymentreceipt.paymenttypetext;
            DateTime paymentdate;
            if (DateTime.TryParseExact(paymentreceipt.datetimestamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out paymentdate))
            {
                paymentmodel.Date = paymentdate;
            }
            List<ReceiptAccountModel> receiptaccount = new List<ReceiptAccountModel>();
            if (paymentreceipt.receiptaccountlist != null && paymentreceipt.receiptaccountlist.Count() > 0)
            {
                Array.ForEach(paymentreceipt.receiptaccountlist.ToArray(), x => receiptaccount.Add(new ReceiptAccountModel
                {
                    contractaccountnumber = x.contractaccountnumber,
                    amount = decimal.Parse(x.amount),
                    currencykey = x.currencykey,
                    name = x.name
                }));
            }
            int count = TransactionHistoryConfiguration.ReceiptCount();
            paymentmodel.Totalpage = Pager.CalculateTotalPages(receiptaccount.Count(), count);
            paymentmodel.Viewmore = paymentmodel.Totalpage > 1 ? true : false;
            paymentmodel.ReceiptAccounts = receiptaccount.Skip((page - 1) * count).Take(count).ToList();
            paymentmodel.ReceiptDescription = paymentreceipt.receiptdescription;
            return paymentmodel;
        }

        public static EstimateReceiptDetailModel From(paymentDetails paymentdetails)
        {
            EstimateReceiptDetailModel paymentmodel = new EstimateReceiptDetailModel();
            decimal amount = decimal.Zero;
            decimal.TryParse(paymentdetails.amount.Trim(), out amount);
            paymentmodel.Isdocumentnumber = true;
            paymentmodel.Amount = amount;
            paymentmodel.DocumentNumber = paymentdetails.documentnumber;
            paymentmodel.Channel = paymentdetails.channel;
            paymentmodel.Description = paymentdetails.paymentdescription;
            paymentmodel.PaymentType = paymentdetails.paymenttype;
            paymentmodel.ApplicationNumber = paymentdetails.applicationnumber;
            paymentmodel.PaymentMode = paymentdetails.paymentmode;
            DateTime paymentdate;
            if (DateTime.TryParseExact(paymentdetails.timestamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out paymentdate))
            {
                paymentmodel.Date = paymentdate;
            }
            List<ReceiptAccountModel> receiptaccount = new List<ReceiptAccountModel> {
                new ReceiptAccountModel
            {
                amount = amount,
                contractaccountnumber = paymentdetails.accountnumber,
                name = paymentdetails.name
            }
            };

            //int count = TransactionHistoryConfiguration.ReceiptCount();
            paymentmodel.Totalpage = 1;
            paymentmodel.Viewmore = false;
            paymentmodel.ReceiptAccounts = receiptaccount.ToList();

            return paymentmodel;
        }
    }

    public class ReceiptAccountModel
    {
        public decimal amount { get; set; }

        public string contractaccountnumber { get; set; }

        public string currencykey { get; set; }

        public string name { get; set; }
    }

    public enum ReceiptType
    {
        DocumentNumber,
        TransactionId
    }
}