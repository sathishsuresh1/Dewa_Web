using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.USC.Models.SmartCommunications
{
    [Serializable]
    public class ScheduleCallSubmitModel
    {
        public string ShCallName { get; set; }
        public string ShCallDate { get; set; }
        public string ShCallTime { get; set; }
        public string ShCallMobileNO { get; set; }
        public string ShCallEmailAddress { get; set; }
        public string ShCallDescription { get; set; }
    }
}