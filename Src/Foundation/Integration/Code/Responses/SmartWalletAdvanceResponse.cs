using DEWAXP.Foundation.Integration.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class SmartWalletAdvanceResponse : BaseResponse
    {
        public List<AccountsAdvanceDetails> AdvanceDetails { get; set; }
        public List<MessagesList> Messages { get; set; }

        public static SmartWalletAdvanceResponse From(DewaSvc.swalletadvCheck payload)
        {
            SmartWalletAdvanceResponse typedResponse = null;
            List<AccountsAdvanceDetails> lstAdvanceDetails = null;
            List<MessagesList> lstMessages = null;
            int responseCode = 0;
            decimal advanceAmount = decimal.Zero;
            decimal balance = decimal.Zero;

            typedResponse = new SmartWalletAdvanceResponse();

            int.TryParse(payload.responseCode, out responseCode);
            typedResponse.ResponseCode = responseCode;
            typedResponse.Description = payload.description;

            lstAdvanceDetails = new List<AccountsAdvanceDetails>();
            if (payload.subwcheckadv != null)
            {
                foreach (var item in payload.subwcheckadv)
                {
                    Decimal.TryParse(item.advanceAmount, out advanceAmount);
                    Decimal.TryParse(item.balance, out balance);

                    lstAdvanceDetails.Add(new AccountsAdvanceDetails
                    {
                        AccountNumber = item.contractaccount,
                        AdvanceAmount = advanceAmount,
                        Balance = balance,
                        CollectiveCode = item.collective,
                        ContractAccountName = item.contractAccountName,
                        FinalBillCode = item.finalBill
                    });
                }
            }
            if (payload.subdetreturn != null)
            {
                lstMessages = new List<MessagesList>();
                payload.subdetreturn.ToList().ForEach(x => lstMessages.Add(new MessagesList() { AccountNumber = x.contractAccount, Message = x.returnMessage, MessageType = x.returnType }));
            }
            typedResponse.AdvanceDetails = lstAdvanceDetails;
            typedResponse.Messages = lstMessages;
            return typedResponse;
        }
    }

    public class AccountsAdvanceDetails
    {
        private string accountNumber;

        public decimal AdvanceAmount { get; set; }
        public decimal Balance { get; set; }
        public string CollectiveCode { get; set; }
        public bool IsCollectiveAccount
        {
            get { return "X".Equals(CollectiveCode); }
        }
        public string AccountNumber
        {
            get { return DewaResponseFormatter.Trimmer(accountNumber); }
            set { accountNumber = value ?? string.Empty; }
        }
        public string ContractAccountName { get; set; }
        public string FinalBillCode { get; set; }
        public bool IsFinalBill
        {
            get { return "X".Equals(FinalBillCode); }
        }
    }

    public class MessagesList
    {
        private string accountNumber;
        public string AccountNumber
        {
            get { return DewaResponseFormatter.Trimmer(accountNumber); }
            set { accountNumber = value ?? string.Empty; }
        }
        public string Message { get; set; }
        public string MessageType { get; set; }
    }
}
