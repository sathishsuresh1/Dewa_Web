using System;
using System.Collections.Generic;
using System.Linq;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Content.Models.Payment;

namespace DEWAXP.Foundation.Content.Models.Bills
{
	[Serializable]
	public class ViewFriendsBillsModel
	{
	    public ViewFriendsBillsModel()
	    {
	        Bills = new List<Account>();
	    }

	    public string SearchCriteria { get; set; }

        public IList<Account> Bills { get; set; }
        public PaymentMethod paymentMethod { get; set; }

		public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }

        public bool MultipleBillsFound
	    {
	        get { return Bills.Count() > 1; }
	    }

	    public Account First
	    {
            get { return Bills.FirstOrDefault(); }
	    }

	    public decimal Total
	    {
            get { return Bills.Sum(bill => bill.Balance); }
	    }

	    public decimal TotalElectedPayments
	    {
            get { return Math.Abs(Bills.Sum(b => b.ElectedPaymentAmount)); }
	    }

	    public static ViewFriendsBillsModel From(string accountOrPremiseNumber, BillEnquiryResponse response)
	    {
	        return new ViewFriendsBillsModel()
	        {
                SearchCriteria = accountOrPremiseNumber,
	            Bills = response.ContainsMultipleAccounts ? 
                    response.Bills.Select(Account.From).ToList() : new List<Account>(new[] {Account.From(response)})
	        };
	    }

	}
}