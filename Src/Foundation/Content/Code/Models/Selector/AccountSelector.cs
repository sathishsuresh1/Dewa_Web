using System.Collections.Generic;
using System.Linq;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.Content.Models
{
    public class AccountSelector : AccountList
    {
        private int _minSelection = 1;
        private int _maxSelection = 10;
        private string _selectedAccountNumber;
        private bool _excludeInactiveAccounts = true;
        private int _pageSize = 20;
        public string SelectedAccountNumber
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_selectedAccountNumber))
                {
                    var selected = this.FirstOrDefault(acc => acc.Selected);
                    if (selected != null)
                    {
                        _selectedAccountNumber = selected.AccountNumber;
                    }
                }
                return _selectedAccountNumber;
            }
            set { _selectedAccountNumber = value; }
        }

        [SitecoreField("Allow multi-selection")]
        public virtual bool MultiSelect { get; set; }

        [SitecoreField("Allow account selection")]
        public virtual bool AccountSelectionEnabled { get; set; }

        [SitecoreField("Submit on confirmation")]
        public virtual bool SubmitOnConfirm { get; set; }

        [SitecoreField("Show account details")]
        public virtual bool ShowAccountDetails { get; set; }

        [SitecoreField("Show account details Link")]
        public virtual bool ShowAccountDetailsLink { get; set; }

        [SitecoreField("Label text")]
        public virtual string Label { get; set; }

        [SitecoreField("Minimum selected accounts")]
        public virtual int MinSelection
        {
            get { return _minSelection; }
            set { _minSelection = value; }
        }

        [SitecoreField("Maximum selected accounts")]
        public virtual int MaxSelection
        {
            get { return _maxSelection; }
            set { _maxSelection = value; }
        }

        /// <summary>
        /// Used for pulling consuming the webservice that returns all active accounts and previous 3 months final bill account
        /// </summary>
        [SitecoreField("Secondary Datasource")]
        public virtual bool SecondaryDatasource { get; set; }

        [SitecoreField("Electric Vehicle")]
        public virtual bool ElectricVehicleAccount { get; set; }

        [SitecoreField("Residential")]
        public virtual bool ResidentialAccount { get; set; }

        [SitecoreField("Non-residential")]
        public virtual bool NonResidentialAccount { get; set; }

        [SitecoreField("Records Per Page")]
        public virtual int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }
        [SitecoreField("Owner")]
        public virtual bool Owner { get; set; }

        [SitecoreField("Tenant")]
        public virtual bool Tenant { get; set; }

        [SitecoreField("DashboardPage")]
        public virtual bool DashboardPage { get; set; }

        public virtual bool ExcludeInactiveAccounts
        {
            get { return _excludeInactiveAccounts; }
            set { _excludeInactiveAccounts = value; }
        }

        public override IEnumerable<Account> Selection
        {
            get
            {
                if (!MultiSelect)
                {
                    return new[] { Accounts.FirstOrDefault(acc => acc.AccountNumber.Equals(SelectedAccountNumber)) };
                }
                return Accounts.Where(a => a.Selected);
            }
        }

		/// <summary>
		/// Added Clearance Certificate Field for Account Selector
		/// </summary>
		[SitecoreField("Notification Code")]
		public virtual string NotificationCode { get; set; }

        [SitecoreField("Service Flag")]
        public virtual string ServiceFlag { get; set; }
        [SitecoreField("Selected Account CacheKey")]
        public virtual string SelectedAccountCacheKey { get; set; }

    }
}