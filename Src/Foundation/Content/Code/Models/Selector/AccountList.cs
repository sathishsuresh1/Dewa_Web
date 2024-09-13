using Sitecore.ContentSearch.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using X.PagedList;

namespace DEWAXP.Foundation.Content.Models
{
    public abstract class AccountList : IEnumerable<Account>
    {
        protected AccountList()
        {
            Accounts = new List<Account>();
        }

        public IList<Account> Accounts { get; protected set; }

        #region Testing for applying paging

        public IPagedList<Account> PagedAccounts { get; set; }
        public IPagedList<Account>[] PagedAccountsArray { get; set; }

        #endregion Testing for applying paging

        public Account Primary
        {
            get { return Accounts.FirstOrDefault(a => a.Primary); }
        }

        public Account First
        {
            get { return Accounts.FirstOrDefault(); }
        }

        public Account FirstSelected
        {
            get { return Accounts.FirstOrDefault(acc => acc.Selected); }
        }

        public virtual IEnumerable<Account> Selection
        {
            get
            {
                return Accounts.Where(a => a.Selected);
            }
        }

        public decimal Total
        {
            get { return Accounts.Sum(acc => acc.Balance); }
        }

        public decimal TotalElectedPaymentAmount
        {
            get { return Selection.Sum(acc => acc.ElectedPaymentAmount); }
        }

        public AccountList With(params Account[] accounts)
        {
            foreach (var account in accounts)
            {
                if (!Accounts.Contains(account))
                {
                    Accounts.Add(account);
                }
            }
            return this;
        }

        public AccountList FilterTo(params string[] accounts)
        {
            if (accounts != null && accounts.Any())
            {
                Accounts = Accounts.RemoveWhere(acc => !accounts.Contains(acc.AccountNumber)).ToList();
            }
            return this;
        }

        public IEnumerator<Account> GetEnumerator()
        {
            return Accounts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}