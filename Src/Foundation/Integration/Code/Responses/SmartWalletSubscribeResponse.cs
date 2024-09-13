using DEWAXP.Foundation.Integration.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class SmartWalletSubscribeResponse : BaseResponse
    {
        public List<SubscribedAccounts> lstSubscribedAccountDetails { get; set; }
        public List<SubscribedAccountsAdvanceDetails> lstSubscribedAccountAdvDetails { get; set; }
        public List<SubscribedAccountsMessages> lstMessages { get; set; }

        public static SmartWalletSubscribeResponse From(DewaSvc.subscribe payload)
        {
            SmartWalletSubscribeResponse parsedresponse = new SmartWalletSubscribeResponse();
            decimal advanceAmount = decimal.Zero;
            decimal balance = decimal.Zero;
            if (payload != null)
            {
                if (payload.parentSubscribeList != null && payload.parentSubscribeList.Count() > 0)
                {
                    parsedresponse.lstSubscribedAccountDetails = new List<SubscribedAccounts>();
                    foreach (var item in payload.parentSubscribeList)
                    {
                        Decimal.TryParse(item.advanceamount, out advanceAmount);
                        Decimal.TryParse(item.balance, out balance);
                        parsedresponse.lstSubscribedAccountDetails.Add(new SubscribedAccounts
                        {
                            AccountNumber = item.contractAccount,
                            AdvanceAmount = advanceAmount,
                            AccountName = item.contractAccountName,
                            Balance = balance,
                            BusinessPartnerNumber = item.businessPartner,
                            CollectiveCode = item.collective,
                            FinalBillCode = item.finalBill,
                            ElectedPaymentAmount = advanceAmount
                        });
                    }
                }

                if (payload.parentSubwalletAdvCheck != null && payload.parentSubwalletAdvCheck.Count() > 0)
                {

                }
                if (payload.subdetreturn != null && payload.subdetreturn.Count() > 0)
                {
                    parsedresponse.lstMessages = new List<SubscribedAccountsMessages>();
                    payload.subdetreturn.ToList().ForEach(x => parsedresponse.lstMessages.Add(new SubscribedAccountsMessages() { AccountNumber = x.contractAccount, Message = x.returnMessage, MessageType = x.returnType }));
                }
            }
            return parsedresponse;
        }
    }
    [Serializable]
    public class SubscribedAccounts
    {
        private string accountNumber;

        public decimal AdvanceAmount { get; set; }
        public decimal Balance { get; set; }
        public DateTime SubscribedOn { get; set; }
        public DateTime UnsubscriedOn { get; set; }
        public string BusinessPartnerNumber { get; set; }
        public string CollectiveCode { get; set; }

        public string AccountName { get; set; }
        public decimal ElectedPaymentAmount { get; set; }
        public bool IsCollectiveAccount
        {
            get { return "X".Equals(CollectiveCode); }
        }
        public string AccountNumber
        {
            get { return DewaResponseFormatter.Trimmer(accountNumber); }
            set { accountNumber = value ?? string.Empty; }
        }
        public string FinalBillCode { get; set; }
        public bool IsFinalBill
        {
            get { return "X".Equals(FinalBillCode); }
        }
        public string RequesterBusinessPartnerNo { get; set; }
        public string RequesterUserID { get; set; }
        public string SessionID { get; set; }
        public string SubscribeCode { get; set; }
        public bool IsSubscribed
        {
            get { return "Y".Equals(SubscribeCode); }
        }
        public string SubscribedAt { get; set; }
        public string UserID { get; set; }
        public bool Selected { get; set; }
    }

    public class SubscribedAccountsAdvanceDetails : AccountsAdvanceDetails
    {

    }
    public class SubscribedAccountsMessages : MessagesList
    {

    }
}
