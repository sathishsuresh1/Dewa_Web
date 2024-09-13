using DEWAXP.Foundation.Content.Models.Payment;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.Content.Models
{
    public class BillSelector : AccountList
    {
	    private bool _excludeInactiveAccounts = true;
        private int _pageSize = 20;

        [SitecoreField("Allow selection")]
        public virtual bool AllowSelection { get; set; }

        [SitecoreField("Allow editing")]
        public virtual bool Editable { get; set; }

        [SitecoreField("Records Per Page")]
        public virtual int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public virtual bool ExcludeInactiveAccounts
	    {
		    get { return _excludeInactiveAccounts; }
		    set { _excludeInactiveAccounts = value; }
	    }

        public PaymentMethod paymentMethod { get; set; }
        public string bankkey { get; set; }
        public string details { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }
}