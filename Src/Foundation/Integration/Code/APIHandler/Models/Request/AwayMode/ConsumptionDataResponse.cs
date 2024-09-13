using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.AwayMode
{
    public class ConsumptionDataResponse
    {
        public string contractAccount { get; set; }
        public string date { get; set; }
        public string deactivateUrl { get; set; }
        public string description { get; set; }
        public string endDate { get; set; }
        public string extendUrl { get; set; }
        public string requestNo { get; set; }
        public string responseCode { get; set; }
        public string startDate { get; set; }
        public List<DataDetail> electricityDataList { get; set; }
        public List<DataDetail> waterDataList { get; set; }
    }


    public class DataDetail
    {
        public double? consumption { get; set; }
        public string date { get; set; }
    }
}
