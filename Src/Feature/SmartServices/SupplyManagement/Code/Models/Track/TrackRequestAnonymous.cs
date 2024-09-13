using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Models.Track
{
    public class TrackRequestAnonymous
    {
        public string AccountOrRequestNumber { get; set; }
        public string AccountNumber { get; set; }
        public string BusinessPartnerNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string MaskedMobileNumber { get; set; }
        public string MaskedEmailAddress { get; set; }
        public string Flag { get; set; }
        public string SelectedOptions { get; set; }
        public string NotificationNumber { get; set; }
        public string NotificationType { get; set; }
        public string NotificationShortText { get; set; }
        public string EvCardNo { get; set; }
        public string EvNotification { get; set; }
        public string EvStatus { get; set; }
        public string EvStatusDescription { get; set; }
        public List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.AnonymousTrack.TrackRequests> TrackList { get; set; }
        public List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.AnonymousTrack.TrackStatusList> TrackStatusList { get; set; }
    }
}