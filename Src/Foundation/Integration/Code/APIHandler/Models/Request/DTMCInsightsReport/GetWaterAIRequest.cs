using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.DTMCInsightsReport
{
    public class GetWaterAIRequest
    {
        public string dsn { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string sessionid { get; set; }
    }
}
