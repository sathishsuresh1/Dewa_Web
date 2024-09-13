using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.meterreading
{
    public class MeterreadingRequest
    {
        //public notificationdtmcinput notificationdtmcinput { get; set; }
        public string userid { get; set; }
        public bool usagetypeW { get; set; }
        public bool usagetypeE { get; set; }
        public string sessionid { get; set; }
        public string vendorid { get; set; }
        public string lang { get; set; }
        public string contractaccountnumber { get; set; }
        public string premisenumber { get; set; }
    }
}
