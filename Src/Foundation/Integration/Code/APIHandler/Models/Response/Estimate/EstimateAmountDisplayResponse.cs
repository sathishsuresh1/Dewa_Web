using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate
{
    public class EstimateAmountDisplayResponse
    {
        public List<CADetailResponse> CA_details { get; set; }
        public string description { get; set; }
        public List<EstimateDetailResponse> estimate_details { get; set; }
        public string responsecode { get; set; }
    }
    public class CADetailResponse
    {
        public string businesspartnername { get; set; }
        public string businesspartnernumber { get; set; }
        public string contractaccountnumber { get; set; }
    }

    public class EstimateDetailResponse
    {
        public string area { get; set; }
        public string bpcity { get; set; }
        public string bppostalcode { get; set; }
        public string bpregion { get; set; }
        public string bpstreet { get; set; }
        public string businesspartner { get; set; }
        public string consultantbpname { get; set; }
        public string consultantbpnumber { get; set; }
        public string consultantemailaddress { get; set; }
        public string contractaccount { get; set; }
        public string contractaccountcity { get; set; }
        public string contractaccountpostalcode { get; set; }
        public string contractaccountregion { get; set; }
        public string contractaccountstreet { get; set; }
        public string customerpurchaseorder { get; set; }
        public string estimatenumber { get; set; }
        public string estimatevalidfromdate { get; set; }
        public string estimatevalidfromtime { get; set; }
        public string estimatevalidtodate { get; set; }
        public string estimatevalidtotime { get; set; }
        public string netvalueamount { get; set; }
        public string netvaluedueamount { get; set; }
        public string netvaluepaidamount { get; set; }
        public string ownername { get; set; }
        public string ownernumber { get; set; }
        public string plot { get; set; }
        public string projectdefination { get; set; }
        public string projectshortdescription { get; set; }
        public string sddocumentcurrency { get; set; }
        public string sddocumentcurrency2 { get; set; }
        public string sdtype { get; set; }
        public string status { get; set; }
        public string statusicon { get; set; }
        public string subsequencesddocument { get; set; }
    }
}
