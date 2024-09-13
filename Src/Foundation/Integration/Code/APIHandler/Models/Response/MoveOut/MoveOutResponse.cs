using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.MoveOut
{
    public class MoveOutResponse
    {
        public List<MoveOutaccountsDetailResponse> accountslist { get; set; }
        public string additionalinformation { get; set; }
        public string attachmentmandatory { get; set; }
        public List<MoveOutBankDetailsOutResponse> banklist { get; set; }
        public string description { get; set; }
        public List<MoveOutDivisionWiseCAResponse> divisionlist { get; set; }
        public List<MoveOutnotificationDetailsResponse> notificationlist { get; set; }
        public string responsecode { get; set; }
        public List<MoveOutTransferAccountsResponse> trnsferlist { get; set; }

    }

    public class MoveOutaccountsDetailResponse
    {
        public string amounttocollect { get; set; }
        public string attachmentmandatory { get; set; }
        public string businesspartnercategory { get; set; }
        public string businesspartnernumber { get; set; }
        public string contractaccountname { get; set; }
        public string contractaccountnumber { get; set; }
        public string creditbalance { get; set; }
        public string currencykey { get; set; }
        public string customerfirstname { get; set; }
        public string customerlastname { get; set; }
        public string customertype { get; set; }
        public string email { get; set; }
        public string emiratesid { get; set; }
        public string idfirstname { get; set; }
        public string idfullname { get; set; }
        public string idlastname { get; set; }
        public string maskedemail { get; set; }
        public string okaccounttransfer { get; set; }
        public string okcheque { get; set; }
        public string okiban { get; set; }
        public string oknorefund { get; set; }
        public string okpaymenttocollect { get; set; }
        public string okwesternunion { get; set; }
        public string nomoveoutpayflag { get; set; }
        public string mobile { get; set; }
        public string maskedmobile { get; set; }
    }

    public class MoveOutBankDetailsOutResponse
    {
        public string bankcountrykey { get; set; }
        public string bankkey { get; set; }
        public string bankname { get; set; }
        public string bptype { get; set; }
    }

    public class MoveOutDivisionWiseCAResponse
    {
        public string billingclass { get; set; }
        public string businesspartner { get; set; }
        public string contractaccountname { get; set; }
        public string contractaccountnickname { get; set; }
        public string contractaccountnumber { get; set; }
        public string finalbillflag { get; set; }
        public string premiseaddress1 { get; set; }
        public string premiseaddress2 { get; set; }
        public string premisenumber { get; set; }
        public string totalamount { get; set; }
        public string formattedtotalamount { get { return !string.IsNullOrWhiteSpace(totalamount) ? Convert.ToDecimal(totalamount).ToString("N") : null; } }

    }

    public class MoveOutnotificationDetailsResponse
    {
        public string businesspartnernumber { get; set; }
        public string contractaccountnumber { get; set; }
        public string message { get; set; }
        public string messagetype { get; set; }
        public string notificationnumber { get; set; }
        public string referencenumber { get; set; }
    }

    public class MoveOutTransferAccountsResponse
    {
        public string contractaccountname { get; set; }
        public string contractaccountnumber { get; set; }
    }
}
