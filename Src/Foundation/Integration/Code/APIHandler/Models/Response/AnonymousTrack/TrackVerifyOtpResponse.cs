using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.AnonymousTrack
{
    public class TrackVerifyOtpResponse
    {
        public string businesspartnernumber { get; set; }
        public string description { get; set; }
        public List<TrackEmailResponse> emaillist { get; set; }
        public List<TrackMobileResponse> mobilelist { get; set; }
        public string responseCode { get; set; }
        public string flag { get; set; }
    }
    public class TrackEmailResponse
    {
        public string maskedemail { get; set; }
        public string unmaskedemail { get; set; }
    }

    public class TrackMobileResponse
    {
        public string maskedmobile { get; set; }
        public string unmaskedmobile { get; set; }
    }
}
