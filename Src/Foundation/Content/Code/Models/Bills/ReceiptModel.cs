using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
using System;
using System.Globalization;

namespace DEWAXP.Foundation.Content.Models.Bills
{
    public class AccountReceiptModel : ReceiptModel
    {
        public string AccountName { get; set; }

	    public bool AccountIsActive { get; set; }

        public bool PhotoAvailable { get; set; }

        public Integration.Enums.BillingClassification BillingClassification { get; set; }

        public static AccountReceiptModel From(Integration.Responses.Receipt receipt, Integration.Responses.AccountDetails account)
	    {
		    return new AccountReceiptModel
		    {
				AccountName = string.IsNullOrWhiteSpace(account.NickName) ? account.AccountName : account.NickName,
				AccountIsActive = account.IsActive,
				PhotoAvailable = account.HasPhoto,
			    AccountNumber = receipt.AccountNumber,
			    Amount = receipt.Amount,
                SuqiaAmount = !string.IsNullOrWhiteSpace(receipt.PaymentGatewayReconStatus) ? decimal.Parse(receipt.PaymentGatewayReconStatus) : 0,
                BusinessPartnerNumber = receipt.BusinessPartnerNumber,
			    DewaTransactionReference = receipt.PaymentGatewayTransactionReference,
			    DegTransactionReference = receipt.ReceiptId,
                NonFormattedDate = receipt.PaymentDate,
			    Date = DateTime.ParseExact(receipt.PaymentDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
		    };
	    }

        public static AccountReceiptModel From(DEWAXP.Foundation.Integration.Responses.Receipt receipt,Account account)
        {
            return new AccountReceiptModel
            {
                AccountName = string.IsNullOrWhiteSpace(account.NickName) ? account.Name : account.NickName,
                AccountIsActive = account.Active,
                PhotoAvailable = account.HasPhoto,
                AccountNumber = receipt.AccountNumber,
                Amount = receipt.Amount,
                SuqiaAmount = !string.IsNullOrWhiteSpace(receipt.PaymentGatewayReconStatus) ? decimal.Parse(receipt.PaymentGatewayReconStatus) : 0,
                BusinessPartnerNumber = receipt.BusinessPartnerNumber,
                DewaTransactionReference = receipt.PaymentGatewayTransactionReference,
                DegTransactionReference = receipt.ReceiptId,
                NonFormattedDate = receipt.PaymentDate,
                Date = DateTime.ParseExact(receipt.PaymentDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                BillingClassification = account.BillingClassification,
            };
        }
    }

    public class ReceiptModel : TransactionModel
    {
        public ReceiptModel()
        {
            Type = TransactionType.Receipt;
        }

        public string DewaTransactionReference { get; set; }

        public string BusinessPartnerNumber { get; set; }

        public string AccountNumber { get; set; }

        public string ReceiptAccountName { get; set; }

        public string ReceiptId { get; set; }

        public string paymenttype { get; set; }

        public string timestamp { get; set; }

        public string channel { get; set; }
        public string documentnumber { get; set; }

        public bool IsTransactionId { get; set; }

        public bool noreceipt { get; set; }

        //public static ReceiptModel From(paymentDetails payment) => new ReceiptModel
        //{
        //    AccountNumber = payment.accountnumber,
        //    Amount = decimal.Parse(payment.amount),
        //    NonFormattedDate = payment.timestamp,
        //    Date = DateTime.ParseExact(payment.timestamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //    ReceiptId = !string.IsNullOrEmpty(payment.transactionid) ? payment.transactionid : payment.documentnumber,
        //    DegTransactionReference = payment.transactionid,
        //    IsTransactionId = !string.IsNullOrEmpty(payment.transactionid) ? true : false,
        //    paymenttype = payment.paymenttype,
        //    timestamp = payment.timestamp,
        //    channel = payment.channel,
        //    documentnumber = payment.documentnumber,
        //    noreceipt = payment.noreceipt.Equals("X") ? false : true
        //};

        public static ReceiptModel FromAPI(Paymentlist payment) => new ReceiptModel
        {
            AccountNumber = payment.Accountnumber,
            Amount = decimal.Parse(payment.Amount),
            NonFormattedDate = payment.Timestamp,
            Date = DateTime.ParseExact(payment.Timestamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            ReceiptId = !string.IsNullOrEmpty(payment.Transactionid) ? payment.Transactionid : payment.Documentnumber,
            DegTransactionReference = payment.Transactionid,
            IsTransactionId = !string.IsNullOrEmpty(payment.Transactionid) ? true : false,
            paymenttype = payment.Paymenttype,
            timestamp = payment.Timestamp,
            channel = payment.Channel,
            documentnumber = payment.Documentnumber,
            noreceipt = !string.IsNullOrWhiteSpace(payment.Noreceipt)&& payment.Noreceipt.Equals("X") ? false : true
        };

        public static ReceiptModel From(DEWAXP.Foundation.Integration.Responses.Receipt receipt)
        {
            return new ReceiptModel
            {
                AccountNumber = receipt.AccountNumber,
                Amount = receipt.Amount,
                SuqiaAmount = !string.IsNullOrWhiteSpace(receipt.PaymentGatewayReconStatus) ?decimal.Parse(receipt.PaymentGatewayReconStatus) :0,
                BusinessPartnerNumber = receipt.BusinessPartnerNumber,
                DewaTransactionReference = receipt.DewaTransactionReference,
                DegTransactionReference = receipt.PaymentGatewayTransactionReference,
				ReceiptId = receipt.ReceiptId,
                NonFormattedDate = receipt.PaymentDate,
                Date = DateTime.ParseExact(receipt.PaymentDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
            };
        }
    }
}