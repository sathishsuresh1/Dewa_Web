using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GeneralServices.Models.ManageAlerts
{
    public class Unsubscribe
    {
        public string emailid { get; set; }
        public string mobile { get; set; }
        public string emailflag { get; set; }
        public string mobileflag { get; set; }
        public string contractaccount { get; set; }
        public string selectedreason { get; set; }
        public string Title { get; set; }
        public string description { get; set; }
        public bool verifysuccess { get; set; }
        public bool success { get; set; }
        public string subscribelink { get; set; }
    }
}