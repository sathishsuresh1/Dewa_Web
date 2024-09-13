using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class AccountSummary
    {
        private string _accountNumber;
        private string _premiseNumber;
        private string _businessPartnerNumber;
        /// <summary>
        /// Define account category
        /// </summary>
        [JsonProperty("accountcategory")]
        public string AccountCategory { get; set; }
        /// <summary>
        /// Define account category text
        /// </summary>
        [JsonProperty("accountcategorytext")]
        public string Category { get; set; }
        /// <summary>
        /// Define account name
        /// </summary>
        [JsonProperty("contractaccountname")]
        public string Name { get; set; }
        /// <summary>
        /// Define billingclass
        /// </summary>
        [JsonProperty("billingclass")]
        public string BillingClassCode { get; set; }
        /// <summary>
        /// Define bpname
        /// </summary>
        [JsonProperty("bpname")]
        public string BPName { get; set; }
        /// <summary>
        /// Define businesspartner
        /// </summary>
        [JsonProperty("businesspartner")]
        public string BusinessPartnerNumber
        {
            get { return DewaResponseFormatter.Trimmer(_businessPartnerNumber); }
            set { _businessPartnerNumber = value ?? string.Empty; }
        }
        /// <summary>
        /// Define contractaccount
        /// </summary>
        [JsonProperty("contractaccount")]
        public string AccountNumber
        {
            get { return DewaResponseFormatter.Trimmer(_accountNumber); }
            set { _accountNumber = value ?? string.Empty; }
        }
        /// <summary>
        /// Define customertype
        /// </summary>
        [JsonProperty("customertype")]
        public string CustomerType { get; set; }
        /// <summary>
        /// Define legacycccount
        /// </summary>
        [JsonProperty("legacycccount")]
        public string LegacyAccountNumber { get; set; }
        /// <summary>
        /// Define location
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }
        /// <summary>
        /// Define medicalissue
        /// </summary>
        [JsonProperty("medicalissue")]
        public bool Medical { get; set; }
        /// <summary>
        /// Define nickname
        /// </summary>
        [JsonProperty("nickname")]
        public string Nickname { get; set; }
        /// <summary>
        /// Define notificationtext
        /// </summary>
        [JsonProperty("notificationtext")]
        public string NotificationNumber { get; set; }
        /// <summary>
        /// Define photoca
        /// </summary>
        [JsonProperty("photoca")]
        public bool PhotoIndicator { get; set; }
        public bool HasPhoto
        {
            get { return PhotoIndicator; }
        }
        public bool IsActive
        {
            get
            {
                return true;
            }
        }
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
        /// <summary>
        /// Define pod
        /// </summary>
        [JsonProperty("pod")]
        public bool POD { get; set; }
        /// <summary>
        /// Define premisenumber
        /// </summary>
        [JsonProperty("premisenumber")]
        public string PremiseNumber
        {
            get { return DewaResponseFormatter.Trimmer(_premiseNumber); }
            set { _premiseNumber = value ?? string.Empty; }
        }
        /// <summary>
        /// Define premisetype
        /// </summary>
        [JsonProperty("premisetype")]
        public string PremiseType { get; set; }
        /// <summary>
        /// Define seniorcitizen
        /// </summary>
        [JsonProperty("seniorcitizen")]
        public bool Senior { get; set; }
        /// <summary>
        /// Define street
        /// </summary>
        [JsonProperty("street")]
        public string Street { get; set; }
        /// <summary>
        /// Define xcord
        /// </summary>
        [JsonProperty("xcord")]
        public string XCordinate { get; set; }
        /// <summary>
        /// Define ycord
        /// </summary>
        [JsonProperty("ycord")]
        public string YCordinate { get; set; }

        
        
    }
    public class ContractAccountListResponse
    {
        [JsonProperty("customertype")]
        public string customertype { get; set; }

        public bool isexpocustomer
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(customertype))
                {
                    if (customertype.Equals("X"))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        [JsonProperty("contractaccounts")]
        public List<AccountSummary> ContractAccounts { get; set; }
        [JsonProperty("responsecode")]
        public string responseCode { get; set; }
        [JsonProperty("description")]
        public string responseMessage { get; set; }
    }
}
