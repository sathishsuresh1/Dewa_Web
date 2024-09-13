using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Events.Models.Events
{
    public class SessionModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string EventCode { get; set; }
        public string SessionName { get; set; }
        public bool IsValidEmail { get; set; } = false;
        public bool IsValidCode { get; set; } = false;
        public string Message { get; set; }
    }
}