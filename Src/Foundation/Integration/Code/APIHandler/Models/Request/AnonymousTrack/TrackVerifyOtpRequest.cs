using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack
{
    public class TrackVerifyOtpRequest
    {
        public string mode { get; set; }
        public string lang { get; set; }
        public string sessionid { get; set; }
        public string reference { get; set; }
        public string prtype { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string contractaccountnumber { get; set; }
        public string businesspartner { get; set; }
        public string otp { get; set; }
    }
}
