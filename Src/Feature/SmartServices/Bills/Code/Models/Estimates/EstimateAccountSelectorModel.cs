using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    [Serializable]
    public class EstimateAccountSelectorModel
    {
        public string SearchText { get; set; }
        public string SelectedContractAccNo { get; set; }
        public bool OnlyPartailList { get; set; }
        public List<EstimateAccountDetail> EstimateAccountSelectorList { get; set; }

        public List<EstimateAccountDetail> BindFrom(List<CADetailResponse> list,string accountNo ="")
        {
            List<EstimateAccountDetail> returnList = new List<EstimateAccountDetail>();
            foreach (var item in list)
            {
                returnList.Add(new EstimateAccountDetail()
                {
                    BusinessPartnerName = item.businesspartnername,
                    BusinessPartnerNo = item.businesspartnernumber,
                    ContractAccNo = item.contractaccountnumber,
                    IsSelected = Convert.ToBoolean(item.contractaccountnumber == accountNo),
                });
            }
            return returnList;
        }


    }
    public class EstimateAccountDetail
    {
        public string BusinessPartnerNo { get; set; }
        public string BusinessPartnerName { get; set; }
        public string ContractAccNo { get; set; }
        public bool IsSelected { get; set; }
    }
   
}