using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DEWAXP.Foundation.Integration.DewaSvc;
using X.PagedList;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    public class EstimateHistoryData : EstimateRequest
    {
        public EstimateHistoryData()
        {
        }
        public List<CADetailResponse> ContractAccountDetails { get; set; }
        public IPagedList<CADetailResponse> PagedContractAccountDetails { get; set; }
        public List<EstimateDetailResponse> EstimateDetails { get; set; }
        public IPagedList<EstimateDetailResponse> PagedEstimateDetails { get; set; }
        public string[] EstimateNoList { get; set; }
        public string SessionToken { get; set; }
    }

}