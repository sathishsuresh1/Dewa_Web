using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.SupplyManagement.MoveOut
{
    public class SendOtpRequest
    {
        public string mobile { get; set; }
        public string email { get; set; }
        public string bpno { get; set; }
        public string type { get; set; }
        public string Otp { get; set; }
        public string mode { get; set; }
        public string reqId { get; set; }
        public string prtype { get; set; }
        public string option { get; set; }
    }
}