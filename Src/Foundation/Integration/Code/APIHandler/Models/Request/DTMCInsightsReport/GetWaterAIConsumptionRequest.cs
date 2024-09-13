using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.DTMCInsightsReport
{
    public class GetWaterAIConsumptionRequest
    {
        public string sessionid { get; set; }
        public string startdate { get; set; }
        public string premise { get; set; }
        public string enddate { get; set; }
        public string usagetype { get; set; }

    }
}
