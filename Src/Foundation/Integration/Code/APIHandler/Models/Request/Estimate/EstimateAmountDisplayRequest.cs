using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate
{
    public class EstimateAmountDisplayRequest
    {
        public EstimateDetailsRetrieve EstimateDetailsRetrieve { get; set; }
    }
    public class EstimateDetailsRetrieve
    {
        public string appidentifier { get; set; }
        public string appversion { get; set; }
        public string contractaccountnumber { get; set; }
        public string enddate { get; set; }
        public string indicator { get; set; }
        public string mobileosversion { get; set; }
        public string lang { get; set; }
        public string projectdefination { get; set; }
        public string sdnumber { get; set; }
        public string startdate { get; set; }
        public string sessionid { get; set; }
        public string vendorid { get; set; }
    }
}
