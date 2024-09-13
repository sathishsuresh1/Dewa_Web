using DEWAXP.Foundation.Integration.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class AccountDetails
    {
        private string _businessPartnerNumber;
        private string _premiseNumber;
        private string _accountNumber;

        [JsonProperty("outstandingelectricity")]
        public decimal Electricity { get; set; }

        [JsonProperty("outstandingwater")]
        public decimal Water { get; set; }

        [JsonProperty("outstandingsewerage")]
        public decimal Sewerage { get; set; }

        [JsonProperty("outstandingdm")]
        public decimal DM { get; set; }

        [JsonProperty("outstandingcooling")]
        public decimal Cooling { get; set; }

        [JsonProperty("outstandinghousing")]
        public decimal Housing { get; set; }

        [JsonProperty("outstandingothers")]
        public decimal Others { get; set; }

        [JsonProperty("outstandingtotal")]
        public decimal Balance { get; set; }

        [JsonProperty("accountcategorytext")]
        public string Category { get; set; }

        [JsonProperty("accountcategory")]
        public string AccountCategory { get; set; }

        [JsonProperty("paymentunderprocessing")]
        public string PayProcess { get; set; }

        [JsonProperty("name")]
        public string AccountName { get; set; }

        [JsonProperty("legacycccount")]
        public string CustomerPremiseNumber { get; set; }

        [JsonProperty("premisenumber")]
        public string PremiseNumber
        {
            get { return !string.IsNullOrEmpty(_premiseNumber) ? _premiseNumber.TrimStart('0') : string.Empty; }
            set { _premiseNumber = value ?? string.Empty; }
        }

        [JsonProperty("nickname")]
        public string NickName { get; set; }

        [JsonProperty("bpname")]
        public string BPName { get; set; }

        [JsonProperty("connectionstatus")]
        public string AccountStatusCode { get; set; }

        [JsonProperty("finalbill")]
        public bool FinalBillCode { get; set; }

        [JsonProperty("contractaccount")]
        public string AccountNumber
        {
            get { return !string.IsNullOrEmpty(_accountNumber) ? _accountNumber.TrimStart('0') : string.Empty; }
            set { _accountNumber = value ?? string.Empty; }
        }

        [JsonProperty("businesspartner")]
        public string BusinessPartnerNumber
        {
            get { return !string.IsNullOrEmpty(_businessPartnerNumber) ? _businessPartnerNumber.TrimStart('0') : string.Empty; }
            set { _businessPartnerNumber = value ?? string.Empty; }
        }

        [JsonProperty("billingclass")]
        public string BillingClassCode { get; set; }

        [JsonProperty("photoca")]
        public bool PhotoIndicator { get; set; }

        [JsonProperty("customertype")]
        public string CustomerType { get; set; }

        [JsonProperty("notificationtext")]
        public string NotificationNumber { get; set; }

        public bool PartialPaymentPermitted
        {
            get { return !FinalBillCode; }
        }
        public bool IsFinalBill
        {
            get { return FinalBillCode; }
        }
        public bool HasPhoto
        {
            get { return PhotoIndicator; }
        }
        public bool IsActive { get; set; }

        public BillingClassification BillingClass
        {
            get
            {
                if (!string.IsNullOrEmpty(BillingClassCode))
                {
                    return (BillingClassification)int.Parse(BillingClassCode);
                }
                return BillingClassification.Unknown;
            }
        }

        public AccountClassification AccountClass
        {
            get
            {
                if (!string.IsNullOrEmpty(AccountStatusCode))
                {
                    return (AccountClassification)int.Parse(AccountStatusCode);
                }
                return AccountClassification.Unknown;
            }

        }

        public string PremiseType { get; set; }
        public string Street { get; set; }
        public string Location { get; set; }
        public string XCordinate { get; set; }
        public string YCordinate { get; set; }
        public bool POD { get; set; }
        public bool Medical { get; set; }
        public bool Senior { get; set; }
        public bool Isexpocustomer { get; set; }
    }
    public class DetailedContractAccountListResponse
    {
        [JsonProperty("contractaccounts")]
        public List<AccountDetails> ContractAccounts { get; set; }
        [JsonProperty("responsecode")]
        public string responseCode { get; set; }
        [JsonProperty("description")]
        public string responseMessage { get; set; }
    }
}
