using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Premise
{
    public class PremiseDetailsRequest
    {
        public PremiseDetailsRequest()
        {
            PremiseDetailsIN = new PremiseDetailsIN();
        }
        public PremiseDetailsIN PremiseDetailsIN { get; set; }
    }
    public class PremiseDetailsIN
    {
        public string appidentifier { get; set; }
        public string appversion { get; set; }
        public string contractaccount { get; set; }
        public bool dminfo { get; set; }
        public string lang { get; set; }
        public string legacypremisenumber { get; set; }
        public string meternumber { get; set; }
        public bool meterstatusinfo { get; set; }
        public string mobileosver { get; set; }
        public bool outageinfo { get; set; }
        public bool podcustomer { get; set; }
        public string premisenumber { get; set; }
        public bool seniorcitizen { get; set; }
        public string sessionid { get; set; }
        public string userid { get; set; }
        public string vendorid { get; set; }
    }
}
