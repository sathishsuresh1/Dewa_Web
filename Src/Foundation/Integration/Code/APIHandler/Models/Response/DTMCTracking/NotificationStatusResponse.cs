using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.DTMCTracking
{
    public class NotificationStatusResponse
    {
        public string description { get; set; }
        public string responsecode { get; set; }
        public List<WorkDetail> worklist { get; set; }
    }

    public class WorkDetail
    {
        public string additionalinfo1 { get; set; }
        public string additionalinfo2 { get; set; }
        public string additionalinfo3 { get; set; }
        public string additionalinfo4 { get; set; }
        public string currentstatus { get; set; }
        public string date { get; set; }
        public string finalstatus { get; set; }
        public string fromxcord { get; set; }
        public string fromycord { get; set; }
        public string guid { get; set; }
        public string map { get; set; }
        public string notificationnumber { get; set; }
        public string notificationstatustext { get; set; }
        public string status { get; set; }
        public string time { get; set; }
        public string timestamp { get; set; }
        public string toxcord { get; set; }
        public string toycord { get; set; }

    }
}
