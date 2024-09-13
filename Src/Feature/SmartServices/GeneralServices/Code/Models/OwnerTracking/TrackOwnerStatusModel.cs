using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GeneralServices.Models.OwnerTracking
{
    public class TrackOwnerStatusModel
    {
        public string ApplicationNumber { get; set; }
        public string errorDescription { get; set; }
        public string errorCode { get; set; }
        public List<OwnerStatus> OwnerStatusResults { get; set; }
    }
    public class OwnerStatus
    {
        public string ColorCode { get; set; }

        public string DateDescription { get; set; }

        public string Status { get; set; }

        public string StatusDescription { get; set; }

        public string zzDate { get; set; }

       
    }
}