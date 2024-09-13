using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Events.Models.Events
{
    public class PODRegistrationModel
    {
        public Guid? EventID { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Verified { get; set; }
        public string LinkUrl { get; set; }
        
        public string Passcode { get; set; }
        public string Message { get; set; }
    }
}