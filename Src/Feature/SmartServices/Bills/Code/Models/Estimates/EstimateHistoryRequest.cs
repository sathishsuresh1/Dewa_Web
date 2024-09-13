using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    [Serializable]
    public class EstimateRequest
    {
        public string Accountnumber { get; set; }
        public string EstimateNo { get; set; }
        public string Type { get; set; }
        public int ActivePageNo { get; set; }
        public int? PageNo { get; set; }
        public bool IsForceAll { get; set; }
        public string EstimateNoToFilterAccountNo { get; set; }
        public string sdType { get; set; }
    }
}