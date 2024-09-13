using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Dashboard.Models.AwayMode
{
    [Bind(Include = "ca,dmInfo,legacyPremiseNumber,meterNumber,meterStatusInfo,outageInfo,podCustomer,premiseNumber,seniorCitizen")]
    public class PremiseDetailRequestModel
    {
        public string ca { get; set; }
        public bool dmInfo { get; set; }
        public string legacyPremiseNumber { get; set; }
        public string meterNumber { get; set; }
        public bool meterStatusInfo { get; set; }
        public bool outageInfo { get; set; }
        public bool podCustomer { get; set; }
        public string premiseNumber { get; set; }
        public bool seniorCitizen { get; set; }
    }
}