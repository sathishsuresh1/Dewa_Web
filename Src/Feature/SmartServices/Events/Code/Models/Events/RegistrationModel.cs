using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Events.Models.Events
{
    public class RegistrationModel
    {
        public Guid? ID { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Country { get; set; }
        public string Designation { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string EventCode { get; set; }
        public string SessionName { get; set; }
        public string Verified { get; set; }
        public string Institution { get; set; }
        public string LinkUrl { get; set; }
        public List<string> MultipleSessionName { get; set; }
        public List<SelectListItem> SessionList { get; set; }
        public string Message { get; set; }

    }
}