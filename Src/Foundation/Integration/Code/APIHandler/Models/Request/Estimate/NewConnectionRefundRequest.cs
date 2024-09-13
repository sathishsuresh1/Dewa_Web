using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate
{
    public class NewConnectionRefundRequest
    {
        public string appidentifier { get; set; }
        public string applicationnumber { get; set; }
        public string estimatenumber { get; set; }
        public string appversion { get; set; }
        public string email { get; set; }
        public string filecontent { get; set; }
        public string filename { get; set; }
        public string iban { get; set; }
        public string lang { get; set; }
        public string mobile { get; set; }
        public string mobileosversion { get; set; }
        public string mode { get; set; }
        public string reason { get; set; }
        public string refundmode { get; set; }
        public string sessionid { get; set; }
        public string userid { get; set; }
        public string vendorid { get; set; }
    }
}
