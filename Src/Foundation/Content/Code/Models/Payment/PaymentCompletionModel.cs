using DEWAXP.Foundation.Content.Models.Bills;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DEWAXP.Foundation.Content.Models.Payment
{
    public class PaymentCompletionModel
    {
        private decimal _total;
        private string _receipts, _businessPartners, _accounts;

        public PaymentCompletionModel(PaymentContext context, bool succeeded)
        {
            Context = context;
            Succeeded = succeeded;
            Receipts = new List<ReceiptModel>();
        }

        public PaymentContext Context { get; private set; }

        public bool Succeeded { get; private set; }

        public IEnumerable<ReceiptModel> Receipts { get; set; }

        public List<ReceiptModel> LstReceipts { get; set; }

        public string Message { get; set; }

        public string ReceiptIdentifiers
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_receipts))
                {
                    _receipts = string.Join(", ", Receipts.Select(rcpt => rcpt.DegTransactionReference).Distinct());
                }
                return _receipts;
            }
            set { _receipts = value; }
        }

        public string AffectedAccounts
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_accounts))
                {
                    _accounts = string.Join(", ", Receipts.Select(rcpt => rcpt.AccountNumber));
                }
                return _accounts;
            }
            set { _accounts = value; }
        }

        public decimal Total
        {
            get
            {
                if (_total == 0)
                {
                    _total = Receipts.Sum(rcpt => rcpt.Amount);
                }
                return _total;
            }
            set { _total = value; }
        }

        public decimal SuqiaAmount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string BusinessPartners
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_businessPartners))
                {
                    var bpNumbers = Receipts
                        .Where(rcpt => !string.IsNullOrWhiteSpace(rcpt.BusinessPartnerNumber))
                        .Select(rcpt => rcpt.BusinessPartnerNumber.TrimStart('0'));

                    if (bpNumbers.Any())
                    {
                        _businessPartners = string.Join(", ", bpNumbers);
                    }
                    _businessPartners = string.Empty;
                }
                return _businessPartners;
            }
            set { _businessPartners = value; }
        }

        public string DegTransactionId { get; set; }

        public string DewaTransactionId { get; set; }

        public bool ShowAffectedAccounts
        {
            get { return Context == PaymentContext.PayBill; }
        }

        public string ContractAccountNumber { get; set; }
        public string CardNumber { get; set; }
        public string RequestNumber { get; set; }

        public string Epnumber { get; set; }
        public string Eptype { get; set; }
        public bool TayseerHistoryPayment { get; set; } = false;
        public bool pgspending { get; set; }
        public decimal pgsTotal { get; set; }
    }
}