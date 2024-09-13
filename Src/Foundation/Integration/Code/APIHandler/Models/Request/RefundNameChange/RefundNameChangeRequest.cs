using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundNameChange
{
    public class RefundNameChangeRequest
    {
        public string dateofbirth { get; set; }
        public string lang { get; set; }
        public string userid { get; set; }
        public string sessionid { get; set; }
        public string filename { get; set; }
        public string passportnumber { get; set; }
        public string notificationnumber { get; set; }
        public string emiratesid { get; set; }
        public string fullnamenew { get; set; }
        public string mode { get; set; }
        public string appidentifier { get; set; }
        public string attachment { get; set; }
        public string nationality { get; set; }
        public string appversion { get; set; }
        public string fullnameold { get; set; }
        public string vendorid { get; set; }
        public bool isuae { get; set; }
    }
}
