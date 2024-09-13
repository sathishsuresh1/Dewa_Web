using System;

namespace DEWAXP.Feature.GeneralServices.Models.SupportKiosk
{

    [Serializable]
    public class TrackRequestDetails
    {
        public string RequestNumber { get; set; }
        public string RequestDate { get; set; }
        public string RequestType { get; set; }
        public string RequestStatus { get; set; }
        public string AccountNumber { get; set; }
        public string MobileNumber { get; set; }

    }
}