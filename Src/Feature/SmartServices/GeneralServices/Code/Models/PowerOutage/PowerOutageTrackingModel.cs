using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GeneralServices.Models.PowerOutage
{
    public class PowerOutageTrackingModel
    {
        public string NotificationNo { get; set; }
        public bool IsSearched { get; set; }
        public List<PowerOutageTackItem> TrackingData { get; set; }
    }

    public class PowerOutageTackItem
    {
        public string NotificationNo { get; set; }
        public string AccountNo { get; set; }
        public string TypeOfWork { get; set; }
        public string PowerInterruption { get; set; }
        public string PurposeOfWork { get; set; }
        public string IsolationPoint { get; set; }
        public string DEWASubStationNumber { get; set; }
        public string CompanyName { get; set; }
        public string CustomerAuthorizedPersonName { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string EmailID { get; set; }
        public string MobileNumber { get; set; }
        public string Status { get; set; }
    }
}