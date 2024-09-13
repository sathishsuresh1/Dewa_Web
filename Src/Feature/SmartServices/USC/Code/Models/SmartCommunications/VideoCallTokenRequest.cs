using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.USC.Models.SmartCommunications
{
    [Serializable]
    public class VideoCallTokenRequest
    {
        public string NextToken { get; set; }
        public string CurrentToken { get; set; }
        public string CustomerMobileNo { get; set; }
        public string WaitingToken { get; set; }
        public string WaitingTokenTxt { get; set; }
        public bool Status { get; set; }
        public int ErrorCode { get; set; } = 0;
    }
}