﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.DTMCTracking
{
    public class NotificationStatusRequest
    {
        public notificationdtmcinput notificationdtmcinput { get; set; }
    }

    public class notificationdtmcinput
    {
        public string appversion { get; set; }
        public string appidentifier { get; set; }
        public string lang { get; set; }
        public string mobileosversion { get; set; }
        public string notificationnumber { get; set; }
        public string sessionid { get; set; }
        public string userid { get; set; }
        public string vendorid { get; set; }

    }
}
