using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GeneralServices.Models.Miscellaneous
{
    public class TrackMiscellaneousModel
    {
      public List<TrackMiscellaneousDetails> trackMiscellaneousDetails { get; set; }
      public string ApplicationNO { get; set; }
    }

    public class TrackMiscellaneousDetails
    {

        public string Status { get; set; }        
        public DateTime AppSubmittedDate { get; set; }
        public DateTime CurrentWorkflowDate { get; set; }        
        public string CodeGroup { get; set; }
        public string CodeGroupCoding { get; set; }
        public string CodeText { get; set; }
        public string Coding { get; set; }
        public string CreateDate { get; set; }
        public string CustomerNumber { get; set; }
        public string GeneralFlag { get; set; }
        public string NotificationCompletedFlag { get; set; }
        public string NotificationNumber { get; set; }
        public string NotificationTime { get; set; }
        public string NotificationType { get; set; }
        public string NotificationtyPetext { get; set; }
        public string PartOfObjects { get; set; }
        public string ShortText { get; set; }        
        public string StatusDate { get; set; }
        public string StatusTime { get; set; }


    }


    public class MiscellaneousTrackAppType
    {

        public static string ElectricityMeterTesting = "0002";
        public static string TransformerOilTesting = "0003";
        public static string MeterTestingProjects = "0001";
        public static string JointerCertification = "0005";
    }


}