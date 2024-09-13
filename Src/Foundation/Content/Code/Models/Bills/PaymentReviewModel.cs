using Sitecore.Globalization;

namespace DEWAXP.Foundation.Content.Models.Bills
{
    public class PaymentReviewModel
    {
        public string[] AccountNumbers { get; set; }

        public decimal[] Amounts { get; set; }

        public decimal[] PartialPayments { get; set; }

        public decimal SelectionTotal { get; set; }

        public string SelectionTotalFormatted
        {
            get { return string.Format("{0} {1}", SelectionTotal, Translate.Text("AED")); }
        }
    }
}