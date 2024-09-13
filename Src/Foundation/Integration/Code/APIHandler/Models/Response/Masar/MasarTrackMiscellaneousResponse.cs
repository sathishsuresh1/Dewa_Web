using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar
{
    public class MasarTrackMiscellaneousResponse
    {
        public string description { get; set; }
        public string responsecode { get; set; }
        public List<TrackMiscellaneousdetailslist> trackdetailslist { get; set; }
    }


    public class TrackMiscellaneousdetailslist
    {
        public string codegroup { get; set; }
        public string codegroupcoding { get; set; }
        public string codetext { get; set; }
        public string coding { get; set; }
        public string createdate { get; set; }
        public string customernumber { get; set; }
        public string generalflag { get; set; }
        public string notificationcompletedflag { get; set; }
        public string notificationnumber { get; set; }
        public string notificationtime { get; set; }
        public string notificationtype { get; set; }
        public string notificationtypetext { get; set; }
        public string partofobjects { get; set; }
        public string shorttext { get; set; }
        public string status { get; set; }
        public string statusdate { get; set; }
        public string statustime { get; set; }
    }
}
