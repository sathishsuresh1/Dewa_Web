using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.AwayMode
{
    public class ManageAwayModeRequest
    {
        public string action { get; set; }
        public string contractAccount { get; set; }
        public string frequency { get; set; }
        public string beginDate { get; set; }
        public string endDate { get; set; }
        public string email { get; set; }
        public string request { get; set; }
        public string loginType { get; set; }
        public string credential { get; set; }
        public string lang { get; set; }
        public string vendor { get; set; }
        public string otp { get; set; }
        public string code { get; set; }
    }
}
