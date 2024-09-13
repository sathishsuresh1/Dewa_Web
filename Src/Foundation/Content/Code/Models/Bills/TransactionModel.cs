using System;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Foundation.Content.Models.Bills
{
    public abstract class TransactionModel
    {
        public DateTime Date { get; set; }

        public string NonFormattedDate { get; set; }

        public string FormattedDate => Date.ToString("dd MMM yyyy", SitecoreX.Culture);

        public string FormattedDateTime => Date.ToString("dd MMM yyyy | HH:mm:ss", SitecoreX.Culture);

        public decimal Amount { get; set; }
        public decimal SuqiaAmount { get; set; }

        public string DegTransactionReference { get; set; }
        
        public TransactionType Type { get; protected set; }

        public bool IsInvoice => Type == TransactionType.Invoice;

        public bool IsReceipt => Type == TransactionType.Receipt;
    }
}