using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarTrackMiscellaneousRequest
    {
        public TrackMiscellaneousinput trackinput { get; set; }
    }


    public class TrackMiscellaneousinput
    {
        public string bpnumber { get; set; }
        public string lang { get; set; }
        public string sessionid { get; set; }
        public string userid { get; set; }
        public string contractaccountnumber { get; set; }
        public string notificationnumber { get; set; }
        public string servicecode { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string activity { get; set; }
        public string referencenumber { get; set; }
    }
}
