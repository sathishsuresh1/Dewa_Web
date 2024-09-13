using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Payment
{
    public class ContractAccount
    {
        [JsonProperty("accountnumber")]
        public string accountnumber;

        [JsonProperty("amount")]
        public string amount;
    }

    public class SamsungPayRequest
    {
        [JsonProperty("walletin")]
        public Walletin walletin;
    }

    public class Walletin
    {
        [JsonProperty("contractaccountnumber")]
        public string contractaccountnumber;

        [JsonProperty("appversion")]
        public string appversion;

        [JsonProperty("sessionid")]
        public string sessionid;

        [JsonProperty("cardtoken")]
        public string cardtoken;

        [JsonProperty("businesspartnernumber")]
        public string businesspartnernumber;

        [JsonProperty("appidentifier")]
        public string appidentifier;

        [JsonProperty("contract_accounts")]
        public List<ContractAccount> contract_accounts;

        [JsonProperty("email")]
        public string email;

        [JsonProperty("payforfriend")]
        public string payforfriend;

        [JsonProperty("ownerbusinesspartnernumber")]
        public string ownerbusinesspartnernumber;

        [JsonProperty("cardtype")]
        public string cardtype;

        [JsonProperty("servicetype")]
        public string servicetype;

        [JsonProperty("estimatenumber")]
        public string estimatenumber;

        [JsonProperty("device")]
        public string device;

        [JsonProperty("vendorid")]
        public string vendorid;

        [JsonProperty("mobile")]
        public string mobile;

        [JsonProperty("clearancetransactionnumber")]
        public List<string> clearancetransactionnumber;

        [JsonProperty("purchaseordernumber")]
        public string purchaseordernumber;

        [JsonProperty("paymenttype")]
        public string paymenttype;

        [JsonProperty("ycordinate")]
        public string ycordinate;

        [JsonProperty("userid")]
        public string userid;

        [JsonProperty("xcordinate")]
        public string xcordinate;

        [JsonProperty("mobileosversion")]
        public string mobileosversion;

        [JsonProperty("totalamount")]
        public string totalamount;

        [JsonProperty("consultantbusinesspartnernumber")]
        public string consultantbusinesspartnernumber;

        [JsonProperty("lang")]
        public string lang;
    }


}
